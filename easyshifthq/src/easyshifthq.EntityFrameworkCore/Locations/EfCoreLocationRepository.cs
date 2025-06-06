using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using easyshifthq.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace easyshifthq.Locations;

public class EfCoreLocationRepository : EfCoreRepository<EasyshifthqDbContext, Location, Guid>, ILocationRepository
{
    public EfCoreLocationRepository(IDbContextProvider<EasyshifthqDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Location>> GetActiveLocationsAsync()
    {
        return await (await GetQueryableAsync())
            .Where(x => x.IsActive)
            .ToListAsync();
    }

    public async Task<List<Location>> GetLocationsInJurisdictionAsync(string jurisdictionCode)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.JurisdictionCode == jurisdictionCode)
            .ToListAsync();
    }

    public async Task<List<Location>> GetLocationsByTimeZoneAsync(string timeZone)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.TimeZone == timeZone)
            .ToListAsync();
    }

    public async Task<(List<Location> Items, int TotalCount)> GetListAsync(
        string? filter = null,
        bool? isActive = null,
        string? timeZone = null,
        string? jurisdictionCode = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = 10)
    {
        var query = await GetQueryableAsync();
        var totalCount = await query.CountAsync();

        // Apply filters
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => 
                x.Name.Contains(filter) || 
                x.Address.Contains(filter))
            .WhereIf(isActive.HasValue, x => x.IsActive == isActive.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(timeZone), x => x.TimeZone == timeZone)
            .WhereIf(!string.IsNullOrWhiteSpace(jurisdictionCode), x => x.JurisdictionCode == jurisdictionCode);

        // Get filtered count

        // Apply sorting
        query = (sorting?.ToLower() switch
        {
            "name" => query.OrderBy(x => x.Name),
            "name desc" => query.OrderByDescending(x => x.Name),
            "address" => query.OrderBy(x => x.Address),
            "address desc" => query.OrderByDescending(x => x.Address),
            "timezone" => query.OrderBy(x => x.TimeZone),
            "timezone desc" => query.OrderByDescending(x => x.TimeZone),
            "jurisdictioncode" => query.OrderBy(x => x.JurisdictionCode),
            "jurisdictioncode desc" => query.OrderByDescending(x => x.JurisdictionCode),
            "isactive" => query.OrderBy(x => x.IsActive),
            "isactive desc" => query.OrderByDescending(x => x.IsActive),
            _ => query.OrderBy(x => x.Name)
        });

        // Apply paging
        var items = await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();

        return (items, totalCount);
    }
}
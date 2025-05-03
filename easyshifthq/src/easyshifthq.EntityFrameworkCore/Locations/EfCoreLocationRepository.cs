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
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<Location>> GetLocationsInJurisdictionAsync(string jurisdictionCode)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.JurisdictionCode == jurisdictionCode)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<Location>> GetLocationsByTimeZoneAsync(string timeZone)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.TimeZone == timeZone)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}
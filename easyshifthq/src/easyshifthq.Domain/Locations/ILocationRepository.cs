using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace easyshifthq.Locations;

public interface ILocationRepository : IRepository<Location, Guid>
{
    Task<List<Location>> GetActiveLocationsAsync();
    Task<List<Location>> GetLocationsInJurisdictionAsync(string jurisdictionCode);
    Task<List<Location>> GetLocationsByTimeZoneAsync(string timeZone);
    Task<(List<Location> Items, int TotalCount)> GetListAsync(
        string? filter = null,
        bool? isActive = null,
        string? timeZone = null,
        string? jurisdictionCode = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = 10);
}
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
}
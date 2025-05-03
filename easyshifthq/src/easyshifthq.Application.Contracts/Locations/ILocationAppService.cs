using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace easyshifthq.Locations;

public interface ILocationAppService : IApplicationService
{
    Task<LocationDto> GetAsync(Guid id);
    Task<List<LocationDto>> GetListAsync();
    Task<List<LocationDto>> GetActiveLocationsAsync();
    Task<List<LocationDto>> GetLocationsInJurisdictionAsync(string jurisdictionCode);
    Task<List<LocationDto>> GetLocationsByTimeZoneAsync(string timeZone);
    Task<LocationDto> CreateAsync(CreateUpdateLocationDto input);
    Task<LocationDto> UpdateAsync(Guid id, CreateUpdateLocationDto input);
    Task DeleteAsync(Guid id);
    Task SetActiveAsync(Guid id, bool isActive);
}
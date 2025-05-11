using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace easyshifthq.Locations;

public interface ILocationAppService : IApplicationService
{
    Task<LocationDto> GetAsync(Guid id);
    Task<PagedResultDto<LocationDto>> GetListAsync(GetLocationListDto input);
    Task<List<LocationDto>> GetActiveLocationsAsync();
    Task<List<LocationDto>> GetLocationsInJurisdictionAsync(string jurisdictionCode);
    Task<List<LocationDto>> GetLocationsByTimeZoneAsync(string timeZone);
    Task<LocationDto> CreateAsync(CreateUpdateLocationDto input);
    Task<LocationDto> UpdateAsync(Guid id, CreateUpdateLocationDto input);
    Task DeleteAsync(Guid id);
    Task SetActiveAsync(Guid id, bool isActive);
}
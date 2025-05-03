using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace easyshifthq.Locations;

public interface ILocationAppService : IApplicationService
{
    Task<List<LocationDto>> GetActiveLocationsAsync();
    Task<LocationDto> GetAsync(Guid id);
}
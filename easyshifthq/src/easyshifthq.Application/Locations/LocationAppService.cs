using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace easyshifthq.Locations;

public class LocationAppService : ApplicationService, ILocationAppService
{
    private readonly ILocationRepository _locationRepository;

    public LocationAppService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<List<LocationDto>> GetActiveLocationsAsync()
    {
        var locations = await _locationRepository.GetActiveLocationsAsync();
        return ObjectMapper.Map<List<Location>, List<LocationDto>>(locations);
    }

    public async Task<LocationDto> GetAsync(Guid id)
    {
        var location = await _locationRepository.GetAsync(id);
        return ObjectMapper.Map<Location, LocationDto>(location);
    }
}
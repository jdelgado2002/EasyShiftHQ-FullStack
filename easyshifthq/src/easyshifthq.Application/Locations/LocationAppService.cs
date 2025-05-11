using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Services;
using easyshifthq.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Linq;

namespace easyshifthq.Locations;

[Authorize(LocationPermissions.Locations.Default)]
public class LocationAppService : ApplicationService, ILocationAppService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IAsyncQueryableExecuter _asyncExecuter;

    public LocationAppService(
        ILocationRepository locationRepository,
        IAsyncQueryableExecuter asyncExecuter)
    {
        _locationRepository = locationRepository;
        _asyncExecuter = asyncExecuter;
    }

    public async Task<LocationDto> GetAsync(Guid id)
    {
        var location = await _locationRepository.GetAsync(id);
        return ObjectMapper.Map<Location, LocationDto>(location);
    }

    public async Task<PagedResultDto<LocationDto>> GetListAsync(GetLocationListDto input)
    {
        var (items, totalCount) = await _locationRepository.GetListAsync(
            filter: input.Filter,
            isActive: input.IsActive,
            timeZone: input.TimeZone,
            jurisdictionCode: input.JurisdictionCode,
            sorting: input.Sorting,
            skipCount: input.SkipCount,
            maxResultCount: input.MaxResultCount
        );

        return new PagedResultDto<LocationDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Location>, List<LocationDto>>(items),
        };
    }

    public async Task<List<LocationDto>> GetActiveLocationsAsync()
    {
        var locations = await _locationRepository.GetActiveLocationsAsync();
        return ObjectMapper.Map<List<Location>, List<LocationDto>>(locations);
    }

    public async Task<List<LocationDto>> GetLocationsInJurisdictionAsync(string jurisdictionCode)
    {
        var locations = await _locationRepository.GetLocationsInJurisdictionAsync(jurisdictionCode);
        return ObjectMapper.Map<List<Location>, List<LocationDto>>(locations);
    }

    public async Task<List<LocationDto>> GetLocationsByTimeZoneAsync(string timeZone)
    {
        var locations = await _locationRepository.GetLocationsByTimeZoneAsync(timeZone);
        return ObjectMapper.Map<List<Location>, List<LocationDto>>(locations);
    }

    [Authorize(LocationPermissions.Locations.Create)]
    public async Task<LocationDto> CreateAsync(CreateUpdateLocationDto input)
    {
        var location = new Location(
            GuidGenerator.Create(),
            input.Name,
            input.Address,
            input.TimeZone,
            input.JurisdictionCode,
            input.Notes
        );

        await _locationRepository.InsertAsync(location);
        return ObjectMapper.Map<Location, LocationDto>(location);
    }

    [Authorize(LocationPermissions.Locations.Edit)]
    public async Task<LocationDto> UpdateAsync(Guid id, CreateUpdateLocationDto input)
    {
        var location = await _locationRepository.GetAsync(id);
        
        location.Update(
            input.Name,
            input.Address,
            input.TimeZone,
            input.JurisdictionCode,
            input.Notes
        );

        await _locationRepository.UpdateAsync(location);
        return ObjectMapper.Map<Location, LocationDto>(location);
    }

    [Authorize(LocationPermissions.Locations.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _locationRepository.DeleteAsync(id);
    }

    [Authorize(LocationPermissions.Locations.ManageActivity)]
    public async Task SetActiveAsync(Guid id, bool isActive)
    {
        var location = await _locationRepository.GetAsync(id);
        location.SetActive(isActive);
        await _locationRepository.UpdateAsync(location);
    }
}
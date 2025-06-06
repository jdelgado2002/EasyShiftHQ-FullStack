using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace easyshifthq.Locations;

public class LocationPagedResultDto : PagedResultDto<LocationDto>
{
    public int FilteredCount { get; set; }

    public LocationPagedResultDto()
    {
    }

    public LocationPagedResultDto(int totalCount, int filteredCount, IReadOnlyList<LocationDto> items)
        : base(totalCount, items)
    {
        FilteredCount = filteredCount;
    }
} 
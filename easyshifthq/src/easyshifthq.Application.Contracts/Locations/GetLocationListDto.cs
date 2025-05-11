using Volo.Abp.Application.Dtos;

namespace easyshifthq.Locations;

public class GetLocationListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public bool? IsActive { get; set; }
    public string? TimeZone { get; set; }
    public string? JurisdictionCode { get; set; }
} 
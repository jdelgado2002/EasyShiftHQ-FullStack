using System;
using Volo.Abp.Application.Dtos;

namespace easyshifthq.Locations;

public class LocationDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string Address { get; set; }
    public bool IsActive { get; set; }
}
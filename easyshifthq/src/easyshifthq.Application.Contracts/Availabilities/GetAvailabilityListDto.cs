using System;
using Volo.Abp.Application.Dtos;

namespace easyshifthq.Availabilities;

public class GetAvailabilityListDto : PagedAndSortedResultRequestDto
{
    public Guid? EmployeeId { get; set; }
    
    public AvailabilityDayOfWeek? DayOfWeek { get; set; }
    
    public DateTime? TimeOffStartDate { get; set; }
    
    public DateTime? TimeOffEndDate { get; set; }
}

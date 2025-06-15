using System;
using Volo.Abp.EventBus;

namespace easyshifthq.Availabilities;

[EventName("easyshifthq.Availability.TimeOffRequested")]
public class TimeOffRequestedEto
{
    public Guid AvailabilityId { get; set; }
    
    public Guid EmployeeId { get; set; }
    
    public string EmployeeName { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public string? Reason { get; set; }
    
    public DateTime CreationTime { get; set; }
}

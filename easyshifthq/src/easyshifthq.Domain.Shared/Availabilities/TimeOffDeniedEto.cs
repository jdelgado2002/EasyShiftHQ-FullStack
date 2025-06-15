using System;
using Volo.Abp.EventBus;

namespace easyshifthq.Availabilities;

[EventName("easyshifthq.Availability.TimeOffDenied")]
public class TimeOffDeniedEto
{
    public Guid AvailabilityId { get; set; }
    
    public Guid EmployeeId { get; set; }
    
    public Guid ApproverId { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public string DenialReason { get; set; }
    
    public DateTime DenialDate { get; set; }
}

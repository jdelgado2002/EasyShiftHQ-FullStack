using System;
using Volo.Abp.Application.Dtos;

namespace easyshifthq.Availabilities;

public class AvailabilityDto : AuditedEntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    
    public AvailabilityDayOfWeek DayOfWeek { get; set; }
    
    public TimeSpan StartTime { get; set; }
    
    public TimeSpan EndTime { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public DateTime? TimeOffStartDate { get; set; }
    
    public DateTime? TimeOffEndDate { get; set; }
    
    public TimeOffApprovalStatus ApprovalStatus { get; set; }
    
    public DateTime? ApprovalDate { get; set; }
    
    public Guid? ApproverId { get; set; }
    
    public string? Reason { get; set; }
    
    public string? DenialReason { get; set; }
}

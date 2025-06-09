using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace easyshifthq.Availabilities;

public class Availability : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    
    public Guid EmployeeId { get; set; }
    
    public AvailabilityDayOfWeek DayOfWeek { get; private set; }
    
    public TimeSpan StartTime { get; private set; }
    
    public TimeSpan EndTime { get; private set; }
    
    public bool IsAvailable { get; private set; }
    
    public DateTime? TimeOffStartDate { get; private set; }
    
    public DateTime? TimeOffEndDate { get; private set; }
    
    public TimeOffApprovalStatus ApprovalStatus { get; private set; }
    
    public DateTime? ApprovalDate { get; private set; }
    
    public Guid? ApproverId { get; private set; }
    
    private string? _reason;
    public string? Reason
    {
        get => _reason;
        private set
        {
            if (value != null)
            {
                _reason = Check.Length(
                    value,
                    nameof(Reason),
                    maxLength: AvailabilityConsts.MaxReasonLength
                );
            }
            else
            {
                _reason = null;
            }
        }
    }
    
    private string? _denialReason;
    public string? DenialReason
    {
        get => _denialReason;
        private set
        {
            if (value != null)
            {
                _denialReason = Check.Length(
                    value,
                    nameof(DenialReason),
                    maxLength: AvailabilityConsts.MaxReasonLength
                );
            }
            else
            {
                _denialReason = null;
            }
        }
    }

    private Availability() { } // For EF Core

    /// <summary>
    /// Creates a new availability slot for a specific day of week
    /// </summary>
    public Availability(
        Guid id,
        Guid employeeId,
        AvailabilityDayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        bool isAvailable)
        : base(id)
    {
        EmployeeId = employeeId;
        SetAvailability(dayOfWeek, startTime, endTime, isAvailable);
    }

    /// <summary>
    /// Creates a new time-off request
    /// </summary>
    public Availability(
        Guid id,
        Guid employeeId,
        DateTime timeOffStartDate,
        DateTime timeOffEndDate,
        string? reason = null)
        : base(id)
    {
        EmployeeId = employeeId;
        SetTimeOff(timeOffStartDate, timeOffEndDate, reason);
    }

    public void SetAvailability(
        AvailabilityDayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        bool isAvailable)
    {
        if (startTime > endTime)
        {
            throw new BusinessException("Availability:EndTimeCannotBeEarlierThanStartTime");
        }

        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        IsAvailable = isAvailable;
        TimeOffStartDate = null;
        TimeOffEndDate = null;
        Reason = null;
    }

    public void SetTimeOff(
        DateTime timeOffStartDate,
        DateTime timeOffEndDate,
        string? reason = null)
    {
        if (timeOffStartDate > timeOffEndDate)
        {
            throw new BusinessException("Availability:EndDateCannotBeEarlierThanStartDate");
        }

        TimeOffStartDate = timeOffStartDate;
        TimeOffEndDate = timeOffEndDate;
        Reason = reason;
        IsAvailable = false;
        DayOfWeek = 0;
        StartTime = TimeSpan.Zero;
        EndTime = TimeSpan.Zero;
    }
    
    /// <summary>
    /// Approves a time-off request
    /// </summary>
    public void ApproveTimeOff(Guid approverId)
    {
        ValidateTimeOffRequest();
        
        if (ApprovalStatus == TimeOffApprovalStatus.Approved)
        {
            throw new BusinessException("Availability:TimeOffRequestAlreadyApproved");
        }
        
        ApproverId = approverId;
        ApprovalStatus = TimeOffApprovalStatus.Approved;
        ApprovalDate = DateTime.Now;
        DenialReason = null;
    }
    
    /// <summary>
    /// Denies a time-off request with a reason
    /// </summary>
    public void DenyTimeOff(Guid approverId, string reason)
    {
        ValidateTimeOffRequest();
        
        if (ApprovalStatus == TimeOffApprovalStatus.Denied)
        {
            throw new BusinessException("Availability:TimeOffRequestAlreadyDenied");
        }
        
        ApproverId = approverId;
        ApprovalStatus = TimeOffApprovalStatus.Denied;
        ApprovalDate = DateTime.Now;
        DenialReason = reason;
    }
    
    /// <summary>
    /// Validates that this is a time-off request
    /// </summary>
    private void ValidateTimeOffRequest()
    {
        if (TimeOffStartDate == null || TimeOffEndDate == null)
        {
            throw new BusinessException("Availability:NotATimeOffRequest");
        }
    }
}

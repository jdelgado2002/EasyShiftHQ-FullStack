using System.Text.Json.Serialization;

namespace easyshifthq.Availabilities;
/// <summary>  
/// Approval workflow states for a time-off request.  
/// </summary>  
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TimeOffApprovalStatus
{
    /// <summary>Awaiting manager action.</summary>  
    Pending = 0,
    /// <summary>Approved by a manager.</summary>  
    Approved = 1,
    /// <summary>Explicitly denied by a manager.</summary>  
    Denied = 2
}
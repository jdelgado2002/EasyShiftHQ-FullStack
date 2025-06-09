using System;
using System.ComponentModel.DataAnnotations;

namespace easyshifthq.Availabilities;

public class SubmitTimeOffRequestDto
{
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [StringLength(AvailabilityConsts.MaxReasonLength)]
    public string? Reason { get; set; }
}

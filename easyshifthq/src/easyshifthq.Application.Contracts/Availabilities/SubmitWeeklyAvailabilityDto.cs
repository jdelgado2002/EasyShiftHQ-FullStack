using System;
using System.ComponentModel.DataAnnotations;

namespace easyshifthq.Availabilities;

public class SubmitWeeklyAvailabilityDto
{
    [Required]
    public AvailabilityDayOfWeek DayOfWeek { get; set; }
    
    [Required]
    public TimeSpan StartTime { get; set; }
    
    [Required]
    public TimeSpan EndTime { get; set; }
    
    public bool IsAvailable { get; set; } = true;
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace easyshifthq.Availabilities;

public class SubmitWeeklyAvailabilityDto : IValidatableObject
{
    [Required]
    public AvailabilityDayOfWeek DayOfWeek { get; set; }
    
    [Required]
    public TimeSpan StartTime { get; set; }
    
    [Required]
    public TimeSpan EndTime { get; set; }
    
    public bool IsAvailable { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "'EndTime' must be later than 'StartTime'",
                new[] { nameof(StartTime), nameof(EndTime) }
            );
        }
    }
}

using System;
using System.Collections.Generic;
using easyshifthq.Availabilities;

namespace easyshifthq.Web.Pages.Availabilities.ViewModels;

public class EmployeeAvailabilityViewModel
{
    public Guid EmployeeId { get; set; }
    
    public List<AvailabilityDto> WeeklySchedule { get; set; }
    
    public List<AvailabilityDto> TimeOffRequests { get; set; }
    
    public EmployeeAvailabilityViewModel()
    {
        WeeklySchedule = new List<AvailabilityDto>();
        TimeOffRequests = new List<AvailabilityDto>();
    }
}

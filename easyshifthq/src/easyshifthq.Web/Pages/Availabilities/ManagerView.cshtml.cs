using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using easyshifthq.Permissions;
using easyshifthq.Web.Pages.Availabilities.ViewModels;
using easyshifthq.Availabilities;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace easyshifthq.Web.Pages.Availabilities;

[Authorize(AvailabilityPermissions.Availabilities.Default)]
public class ManagerViewModel : AbpPageModel
{
    public EmployeeAvailabilityViewModel EmployeeAvailability { get; set; }
    
    [BindProperty]
    [Required]
    [Display(Name = "DenialReason")]
    public string? DenialReason { get; set; }
    
    private readonly IAvailabilityAppService _availabilityAppService;

    public ManagerViewModel(IAvailabilityAppService availabilityAppService)
    {
        _availabilityAppService = availabilityAppService;
        EmployeeAvailability = new EmployeeAvailabilityViewModel();
    }

    public virtual async Task<IActionResult> OnPostApproveAsync(Guid id)
    {
        await _availabilityAppService.ApproveTimeOffRequestAsync(id);
        return RedirectToPage(new { employeeId = EmployeeAvailability.EmployeeId });
    }

    public virtual async Task<IActionResult> OnPostDenyAsync(Guid id, [Required] string reason)
    {
        await _availabilityAppService.DenyTimeOffRequestAsync(id, reason);
        return RedirectToPage(new { employeeId = EmployeeAvailability.EmployeeId });
    }

    public virtual async Task<IActionResult> OnGetAsync(Guid employeeId)
    {
        if (employeeId == Guid.Empty)
        {
            return RedirectToPage("/Availabilities/WeeklyAvailability");
        }

        EmployeeAvailability.EmployeeId = employeeId;
        
        try
        {
            EmployeeAvailability.WeeklySchedule = await _availabilityAppService
                .GetEmployeeWeeklyAvailabilityAsync(employeeId);
                
            EmployeeAvailability.TimeOffRequests = await _availabilityAppService
                .GetEmployeeTimeOffRequestsAsync(employeeId);
            
            return Page();
        }
        catch (UnauthorizedAccessException)
        {
            // If unauthorized, redirect to their own availability
            return RedirectToPage("/Availabilities/WeeklyAvailability");
        }
    }
}

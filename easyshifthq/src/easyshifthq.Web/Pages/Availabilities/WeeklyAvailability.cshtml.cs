using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using easyshifthq.Availabilities;
using easyshifthq.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using System.Collections.Generic;

namespace easyshifthq.Web.Pages.Availabilities;

[Authorize(AvailabilityPermissions.Availabilities.Default)]
public class WeeklyAvailabilityModel : AbpPageModel
{
    private readonly IAvailabilityAppService _availabilityAppService;
    
    public WeeklyAvailabilityModel(IAvailabilityAppService availabilityAppService)
    {
        _availabilityAppService = availabilityAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        if (!CurrentUser.Id.HasValue)
        {
            return Challenge();
        }

        // Validate access through service layer
        try
        {
            await _availabilityAppService.GetEmployeeWeeklyAvailabilityAsync(CurrentUser.Id.Value);
            return Page();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    public virtual async Task<JsonResult> OnGetWeeklyAvailabilityAsync()
    {
        try
        {
            if (!CurrentUser.Id.HasValue)
            {
                return new JsonResult(new { error = "Not authenticated" }) { StatusCode = 401 };
            }

            var availability = await _availabilityAppService.GetEmployeeWeeklyAvailabilityAsync(CurrentUser.Id.Value);
            return new JsonResult(new { success = true, data = availability });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }
}

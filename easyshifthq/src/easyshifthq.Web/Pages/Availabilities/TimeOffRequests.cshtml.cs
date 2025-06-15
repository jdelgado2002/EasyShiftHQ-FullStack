using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using easyshifthq.Availabilities;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace easyshifthq.Web.Pages.Availabilities;

public class TimeOffRequestsModel : AbpPageModel
{
    public Guid? CurrentUserId => CurrentUser.Id;

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}

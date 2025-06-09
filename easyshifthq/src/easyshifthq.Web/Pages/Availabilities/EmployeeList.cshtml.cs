using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp.Identity;
using easyshifthq.Permissions;

namespace easyshifthq.Web.Pages.Availabilities;

[Authorize(AvailabilityPermissions.Availabilities.Approve)]
public class EmployeeListModel : easyshifthqPageModel
{
    private readonly IIdentityUserAppService _identityUserAppService;

    public EmployeeListModel(IIdentityUserAppService identityUserAppService)
    {
        _identityUserAppService = identityUserAppService;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnGetEmployeeListAsync()
    {
        try
        {
            var input = new GetIdentityUsersInput
            {
                MaxResultCount = 1000, // Get a reasonable number of employees
                Sorting = "userName"
            };

            var result = await _identityUserAppService.GetListAsync(input);
            
            return new JsonResult(new
            {
                success = true,
                data = result.Items.Select(user => new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    name = user.Name,
                    surname = user.Surname,
                    displayName = !string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Surname) 
                        ? $"{user.Name} {user.Surname}" 
                        : user.UserName
                }).ToList()
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching employee list");
            return new JsonResult(new
            {
                success = false,
                message = "Error fetching employee list"
            });
        }
    }
}

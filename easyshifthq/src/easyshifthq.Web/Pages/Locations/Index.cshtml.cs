using easyshifthq.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace easyshifthq.Web.Pages.Locations;

[Authorize(LocationPermissions.Locations.Default)]
public class IndexModel : easyshifthqPageModel
{
    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
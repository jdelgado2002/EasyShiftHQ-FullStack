using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using easyshifthq.Locations;

namespace easyshifthq.Web.Pages.Locations;

public class CreateModalModel : easyshifthqPageModel
{
    [BindProperty]
    public CreateUpdateLocationDto Location { get; set; }

    private readonly ILocationAppService _locationAppService;

    public CreateModalModel(ILocationAppService locationAppService)
    {
        _locationAppService = locationAppService;
        Location = new CreateUpdateLocationDto();
    }

    public void OnGet()
    {
        // No need to initialize Location here since it's done in constructor
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _locationAppService.CreateAsync(Location);
        return NoContent();
    }
}
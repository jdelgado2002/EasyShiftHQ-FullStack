using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using easyshifthq.Locations;

namespace easyshifthq.Web.Pages.Locations;

public class CreateModalModel : easyshifthqPageModel
{
    [BindProperty]
    public required CreateUpdateLocationDto Location { get; set; }

    private readonly ILocationAppService _locationAppService;

    public CreateModalModel(ILocationAppService locationAppService)
    {
        _locationAppService = locationAppService;
    }

    public void OnGet()
    {
        Location = new CreateUpdateLocationDto();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _locationAppService.CreateAsync(Location);
        return NoContent();
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using easyshifthq.Locations;

namespace easyshifthq.Web.Pages.Locations;

public class EditModalModel : easyshifthqPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public required CreateUpdateLocationDto Location { get; set; }

    private readonly ILocationAppService _locationAppService;

    public EditModalModel(ILocationAppService locationAppService)
    {
        _locationAppService = locationAppService;
    }

    public async Task OnGetAsync()
    {
        var location = await _locationAppService.GetAsync(Id);
        Location = ObjectMapper.Map<LocationDto, CreateUpdateLocationDto>(location);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _locationAppService.UpdateAsync(Id, Location);
        return NoContent();
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using easyshifthq.Locations;
using Volo.Abp.ObjectMapping;

namespace easyshifthq.Web.Pages.Locations;

[ValidateAntiForgeryToken]
public class EditModalModel : easyshifthqPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateLocationDto Location { get; set; } = new();

    private readonly ILocationAppService _locationAppService;
    private readonly IObjectMapper _objectMapper;

    public EditModalModel(
        ILocationAppService locationAppService,
        IObjectMapper objectMapper)
    {
        _locationAppService = locationAppService;
        _objectMapper = objectMapper;
    }

    public async Task OnGetAsync()
    {
        var location = await _locationAppService.GetAsync(Id);
        Location = _objectMapper.Map<LocationDto, CreateUpdateLocationDto>(location);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _locationAppService.UpdateAsync(Id, Location);
        return NoContent();
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using easyshifthq.Locations;
using Volo.Abp.ObjectMapping;
using Volo.Abp;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using easyshifthq.Permissions;

namespace easyshifthq.Web.Pages.Locations;

[ValidateAntiForgeryToken]
[Authorize(LocationPermissions.Locations.Edit)]
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

    public async Task<IActionResult> OnGetAsync()
    {
        var location = await _locationAppService.GetAsync(Id);
        if (location == null)
        {
            return NotFound();
        }

        var mappedLocation = _objectMapper.Map<LocationDto, CreateUpdateLocationDto>(location);
        if (mappedLocation == null)
        {
            throw new UserFriendlyException(L["Error.Mapping.Failed"]);
        }

        Location = mappedLocation;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {

        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Location == null)
            {
                throw new UserFriendlyException(L["Error.Mapping.Failed"]);
            }
            await _locationAppService.UpdateAsync(Id, Location);
            return NoContent();
        }
        catch (System.Exception ex)
        {
            Logger.LogError(ex, "Failed to update location with ID {LocationId}", Id);

            return BadRequest();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using easyshifthq.Invitations;
using easyshifthq.Locations;
using Volo.Abp;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace easyshifthq.Web.Pages.Invitations;

public class CreateModalModel : easyshifthqPageModel
{
    private readonly IInvitationAppService _invitationAppService;
    private readonly ILocationAppService _locationAppService;

    [BindProperty]
    public CreateInvitationViewModel Invitation { get; set; } = new();

    public List<SelectListItem> Roles { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();

    public CreateModalModel(
        IInvitationAppService invitationAppService,
        ILocationAppService locationAppService)
    {
        _invitationAppService = invitationAppService;
        _locationAppService = locationAppService;
    }

    public async Task OnGetAsync()
    {
        Roles = new List<SelectListItem>
        {
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "Manager", Text = "Manager" },
            new SelectListItem { Value = "Employee", Text = "Employee" }
        };

        var locations = await _locationAppService.GetActiveLocationsAsync();
        Locations = locations
            .Select(l => new SelectListItem(l.Name, l.Id.ToString()))
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var dto = ObjectMapper.Map<CreateInvitationViewModel, CreateInvitationDto>(Invitation);
            if (dto == null)
            {
                throw new UserFriendlyException(L["Error.Mapping.Failed"]);
            }

            await _invitationAppService.CreateAsync(dto);
            return NoContent();
        }
        catch (AutoMapperMappingException ex)
        {
            Logger.LogError(ex, "Mapping failed for invitation creation");
            throw new UserFriendlyException(L["Error.Invitation.Creation.Failed"]);
        }
    }

    public class CreateInvitationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public Guid? LocationId { get; set; }
    }
}
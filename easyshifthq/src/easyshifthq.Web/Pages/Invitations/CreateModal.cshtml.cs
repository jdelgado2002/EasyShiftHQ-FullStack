using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using easyshifthq.Invitations;
using Volo.Abp;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace easyshifthq.Web.Pages.Invitations;

public class CreateModalModel : easyshifthqPageModel
{
    private readonly IInvitationAppService _invitationAppService;

    [BindProperty]
    public CreateInvitationViewModel Invitation { get; set; }

    public List<SelectListItem> Roles { get; set; }
    public List<SelectListItem> Locations { get; set; }

    public CreateModalModel(IInvitationAppService invitationAppService)
    {
        _invitationAppService = invitationAppService;
        Invitation = new CreateInvitationViewModel();
    }

    public void OnGet()
    {
        // TODO: Load roles and locations from their respective services
        Roles = new List<SelectListItem>
        {
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "Manager", Text = "Manager" },
            new SelectListItem { Value = "Employee", Text = "Employee" }
        };

        // Locations will be populated from your locations service
        Locations = new List<SelectListItem>();
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
        public string Email { get; set; }

        [Required]
        [StringLength(128)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(128)]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }

        public Guid? LocationId { get; set; }
    }
}
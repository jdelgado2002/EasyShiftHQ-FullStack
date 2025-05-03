using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using easyshifthq.Invitations;
using easyshifthq.Locations;

namespace easyshifthq.Web.Pages.Team;

public class IndexModel : AbpPageModel
{
    private readonly IInvitationAppService _invitationAppService;
    private readonly ILocationAppService _locationAppService;

    [BindProperty]
    public InviteMemberViewModel InviteMember { get; set; }

    [BindProperty]
    public IFormFile CsvFile { get; set; }

    public List<SelectListItem> Roles { get; private set; }
    public List<SelectListItem> Locations { get; private set; }

    public IndexModel(
        IInvitationAppService invitationAppService,
        ILocationAppService locationAppService)
    {
        _invitationAppService = invitationAppService;
        _locationAppService = locationAppService;
    }

    public async Task OnGetAsync()
    {
        await LoadSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var dto = ObjectMapper.Map<InviteMemberViewModel, CreateInvitationDto>(InviteMember);
            await _invitationAppService.CreateAsync(dto);
            return new OkResult();
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { message = ex.Message });
        }
    }

    public async Task<IActionResult> OnPostBulkInviteAsync()
    {
        if (CsvFile == null || CsvFile.Length == 0)
        {
            return new BadRequestObjectResult(new { message = L["PleaseSelectAFile"].Value });
        }

        try
        {
            using var reader = new StreamReader(CsvFile.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            
            var records = csv.GetRecords<BulkInviteRecord>().ToList();
            
            if (!records.Any())
            {
                return new BadRequestObjectResult(new { message = L["NoValidRecordsFound"].Value });
            }

            var bulkInviteDto = new BulkInvitationDto
            {
                Invitations = records.Select(r => new CreateInvitationDto
                {
                    Email = r.Email,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Role = string.IsNullOrEmpty(r.Role) ? "employee" : r.Role,
                    LocationId = string.IsNullOrEmpty(r.LocationId) ? 
                        (Guid?)null : 
                        Guid.Parse(r.LocationId)
                }).ToList()
            };

            await _invitationAppService.CreateBulkAsync(bulkInviteDto);
            return new OkResult();
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { message = L["ErrorProcessingFile"].Value });
        }
    }

    private async Task LoadSelectListsAsync()
    {
        Roles = new List<SelectListItem>
        {
            new SelectListItem("Admin", "admin"),
            new SelectListItem("Manager", "manager"),
            new SelectListItem("Employee", "employee")
        };

        var locations = await _locationAppService.GetActiveLocationsAsync();
        Locations = locations.Select(l => new SelectListItem(l.Name, l.Id.ToString())).ToList();
    }
}

public class InviteMemberViewModel
{
    [Required]
    [Display(Name = "FirstName")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "LastName")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; }

    [Required]
    [Display(Name = "Location")]
    public Guid LocationId { get; set; }
}

public class BulkInviteRecord
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public string LocationId { get; set; }
}
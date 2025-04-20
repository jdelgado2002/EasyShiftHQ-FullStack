using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using easyshifthq.Invitations;
using CsvHelper;
using System.Globalization;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace easyshifthq.Web.Pages.Invitations;

public class BulkCreateModalModel : easyshifthqPageModel
{
    private readonly IInvitationAppService _invitationAppService;

    [BindProperty]
    public string DefaultRole { get; set; }

    [BindProperty]
    public Guid? DefaultLocationId { get; set; }

    [BindProperty]
    public IFormFile CsvFile { get; set; }

    public List<SelectListItem> Roles { get; set; }
    public List<SelectListItem> Locations { get; set; }

    public BulkCreateModalModel(IInvitationAppService invitationAppService)
    {
        _invitationAppService = invitationAppService;
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
            if (CsvFile == null || CsvFile.Length == 0)
            {
                throw new UserFriendlyException(L["PleaseSelectAFile"]);
            }

            var invitations = new List<CreateInvitationDto>();

            using (var reader = new StreamReader(CsvFile.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CsvInvitationRecord>();
                
                foreach (var record in records)
                {
                    invitations.Add(new CreateInvitationDto
                    {
                        Email = record.Email,
                        FirstName = record.FirstName,
                        LastName = record.LastName,
                        Role = !string.IsNullOrEmpty(record.Role) ? record.Role : DefaultRole,
                        LocationId = DefaultLocationId
                    });
                }
            }

            if (!invitations.Any())
            {
                throw new UserFriendlyException(L["NoValidRecordsFound"]);
            }

            var bulkDto = new BulkInvitationDto { Invitations = invitations };
            await _invitationAppService.CreateBulkAsync(bulkDto);

            return NoContent();
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException(L["ErrorProcessingFile"], ex.Message);
        }
    }

    private class CsvInvitationRecord
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}
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
using easyshifthq.Locations;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Volo.Abp;

namespace easyshifthq.Web.Pages.Invitations;

public class BulkCreateModalModel : easyshifthqPageModel
{
    private readonly IInvitationAppService _invitationAppService;
    private readonly ILocationAppService _locationAppService;

    [BindProperty]
    public string DefaultRole { get; set; } = string.Empty;

    [BindProperty]
    public Guid? DefaultLocationId { get; set; }

    [BindProperty]
    public IFormFile? CsvFile { get; set; }

    public List<SelectListItem> Roles { get; set; }
    public List<SelectListItem> Locations { get; set; }

    public BulkCreateModalModel(
        IInvitationAppService invitationAppService,
        ILocationAppService locationAppService)
    {
        _invitationAppService = invitationAppService;
        _locationAppService = locationAppService;
        
        // Initialize lists
        Roles = new List<SelectListItem>();
        Locations = new List<SelectListItem>();
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
            if (CsvFile == null || CsvFile.Length == 0)
            {
                throw new UserFriendlyException(L["PleaseSelectAFile"]);
            }

            var invitations = ProcessCsvFile(CsvFile);

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

    private List<CreateInvitationDto> ProcessCsvFile(IFormFile file)
    {
        var invitations = new List<CreateInvitationDto>();
        var processedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
            Delimiter = ","
        };

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<CsvInvitationRecord>();

        foreach (var record in records)
        {
            ValidateAndAddInvitation(record, processedEmails, invitations);
        }

        return invitations;
    }

    private void ValidateAndAddInvitation(CsvInvitationRecord record, HashSet<string> processedEmails, List<CreateInvitationDto> invitations)
    {
        if (string.IsNullOrWhiteSpace(record.Email))
        {
            throw new UserFriendlyException(L["EmailIsRequired"]);
        }

        var email = record.Email.Trim().ToLowerInvariant();
        if (!new EmailAddressAttribute().IsValid(email))
        {
            throw new UserFriendlyException(L["InvalidEmailFormat", email]);
        }

        if (!processedEmails.Add(email))
        {
            throw new UserFriendlyException(L["DuplicateEmail", email]);
        }

        Guid? locationId = null;
        if (!string.IsNullOrEmpty(record.LocationId) && 
            Guid.TryParse(record.LocationId, out var parsedLocationId))
        {
            locationId = parsedLocationId;
        }

        invitations.Add(new CreateInvitationDto
        {
            Email = email,
            FirstName = record.FirstName?.Trim() ?? string.Empty,
            LastName = record.LastName?.Trim() ?? string.Empty,
            Role = !string.IsNullOrEmpty(record.Role) ? record.Role.Trim() : DefaultRole,
            LocationId = locationId ?? DefaultLocationId
        });
    }

    private sealed class CsvInvitationRecord
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; init; } = string.Empty;

        [Required(ErrorMessage = "First name is required")] 
        [StringLength(128, ErrorMessage = "First name cannot exceed 128 characters")]
        public string FirstName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(128, ErrorMessage = "Last name cannot exceed 128 characters")] 
        public string LastName { get; init; } = string.Empty;

        [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
        public string Role { get; init; } = string.Empty;

        public string? LocationId { get; init; } = null;
    }
}
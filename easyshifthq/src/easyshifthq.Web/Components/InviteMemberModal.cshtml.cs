using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using easyshifthq.Locations;
using easyshifthq.Web.Pages;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace easyshifthq.Web.Components;

public class InviteMemberModalModel : easyshifthqPageModel
{
    private readonly ILocationAppService _locationAppService;

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

    public List<SelectListItem> Roles { get; private set; }
    public List<SelectListItem> Locations { get; private set; }

    public InviteMemberModalModel(ILocationAppService locationAppService)
    {
        _locationAppService = locationAppService;
    }

    public async Task OnGetAsync()
    {
        Roles = new List<SelectListItem>
        {
            new SelectListItem("Admin", "ADMIN"),
            new SelectListItem("Manager", "MANAGER"),
            new SelectListItem("Employee", "EMPLOYEE")
        };

        var locations = await _locationAppService.GetActiveLocationsAsync();
        Locations = locations.Select(l => new SelectListItem(l.Name, l.Id.ToString())).ToList();
    }
}
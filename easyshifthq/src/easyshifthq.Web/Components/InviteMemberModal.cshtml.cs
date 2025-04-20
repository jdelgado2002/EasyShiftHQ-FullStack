using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using easyshifthq.Locations;
using easyshifthq.Web.Pages;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace easyshifthq.Web.Components;

public class InviteMemberModalModel : easyshifthqPageModel
{
    private readonly ILocationAppService _locationAppService;

    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Role { get; set; }
    
    [Required]
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
            new SelectListItem("Admin", "admin"),
            new SelectListItem("Manager", "manager"),
            new SelectListItem("Employee", "employee")
        };

        var locations = await _locationAppService.GetActiveLocationsAsync();
        Locations = locations.Select(l => new SelectListItem(l.Name, l.Id.ToString())).ToList();
    }
}
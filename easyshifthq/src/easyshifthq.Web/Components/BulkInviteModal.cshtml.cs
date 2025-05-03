using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using easyshifthq.Locations;
using easyshifthq.Web.Pages;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace easyshifthq.Web.Components;

public class BulkInviteModalModel : easyshifthqPageModel
{
    private readonly ILocationAppService _locationAppService;

    public string DefaultRole { get; set; }
    public Guid? DefaultLocation { get; set; }

    public List<SelectListItem> Roles { get; private set; }
    public List<SelectListItem> Locations { get; private set; }

    public BulkInviteModalModel(ILocationAppService locationAppService)
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
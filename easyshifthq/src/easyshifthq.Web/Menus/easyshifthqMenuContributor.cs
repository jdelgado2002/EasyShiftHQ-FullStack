using System.Threading.Tasks;
using easyshifthq.Localization;
using easyshifthq.Permissions;
using easyshifthq.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;

namespace easyshifthq.Web.Menus;

public class EasyshifthqMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<easyshifthqResource>();

        //Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                easyshifthqMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );

        // Add Invitations menu item
        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Invitations",
                l["Invitations"],
                url: "/Invitations",
                icon: "fas fa-envelope",
                order: 3
            )
        );        // Add Location menu item
        if (await context.IsGrantedAsync(LocationPermissions.Locations.Default))
        {
            context.Menu.AddItem(
                new ApplicationMenuItem(
                    "Location",
                    l["LocationManagement"],
                    url: "/Locations",
                    icon: "fas fa-map-marker-alt",
                    order: 4
                )
            );
        }        // Add Availability menu items
        var availabilityMenuItem = new ApplicationMenuItem(
            "Availability",
            l["Availability"],
            icon: "fas fa-calendar-alt",
            order: 5
        )
        .AddItem(
            new ApplicationMenuItem(
                "Availability.Weekly",
                l["WeeklyAvailability"],
                url: "/Availabilities/WeeklyAvailability",
                icon: "fas fa-calendar-week"
            )
        )
        .AddItem(
            new ApplicationMenuItem(
                "Availability.TimeOff",
                l["TimeOffRequests"],
                url: "/Availabilities/TimeOffRequests",
                icon: "fas fa-calendar-times"
            )
        );

        // Add Manager View menu item for users with Approve permission
        if (await context.IsGrantedAsync(AvailabilityPermissions.Availabilities.Approve))
        {
            availabilityMenuItem.AddItem(
                new ApplicationMenuItem(
                    "Availability.ManagerView",
                    l["ManagerView"],
                    url: "/Availabilities/EmployeeList",
                    icon: "fas fa-users-cog"
                )
            );
        }

        context.Menu.AddItem(availabilityMenuItem);

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
    }
}

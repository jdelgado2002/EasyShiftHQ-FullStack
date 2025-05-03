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

public class easyshifthqMenuContributor : IMenuContributor
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

        // Team Management
        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Team",
                l["TeamMembers"],
                url: "/Team",
                icon: "fas fa-users",
                order: 2,
                requiredPermissionName: easyshifthqPermissions.TeamManagement.Default
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
        );

        // Add Location menu item
        if (await context.IsGrantedAsync(LocationPermissions.Locations.Default))
        {
            context.Menu.AddItem(
                new ApplicationMenuItem(
                    "Location",
                    l["LocationManagement"],
                    url: "/Locations",
                    icon: "fas fa-map-marker-alt",
                    order: 3
                )
            );
        }

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
    
        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }
        
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
    }
}

using easyshifthq.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace easyshifthq.Permissions;

public class LocationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var locationGroup = context.AddGroup(
            LocationPermissions.GroupName,
            L("Permission:Location"));

        var locations = locationGroup.AddPermission(
            LocationPermissions.Locations.Default,
            L("Permission:Locations"));

        locations.AddChild(
            LocationPermissions.Locations.Create,
            L("Permission:Locations.Create"));

        locations.AddChild(
            LocationPermissions.Locations.Edit,
            L("Permission:Locations.Edit"));

        locations.AddChild(
            LocationPermissions.Locations.Delete,
            L("Permission:Locations.Delete"));

        locations.AddChild(
            LocationPermissions.Locations.ManageActivity,
            L("Permission:Locations.ManageActivity"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<easyshifthqResource>(name);
    }
}
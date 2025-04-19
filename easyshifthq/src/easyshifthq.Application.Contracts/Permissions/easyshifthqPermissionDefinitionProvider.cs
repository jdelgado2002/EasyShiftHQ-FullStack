using easyshifthq.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace easyshifthq.Permissions;

public class easyshifthqPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(easyshifthqPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(easyshifthqPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<easyshifthqResource>(name);
    }
}

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

        var teamManagement = myGroup.AddPermission(easyshifthqPermissions.TeamManagement.Default, L("Permission:TeamManagement"));
        teamManagement.AddChild(easyshifthqPermissions.TeamManagement.Create, L("Permission:TeamManagement.Create"));
        teamManagement.AddChild(easyshifthqPermissions.TeamManagement.Edit, L("Permission:TeamManagement.Edit"));
        teamManagement.AddChild(easyshifthqPermissions.TeamManagement.Delete, L("Permission:TeamManagement.Delete"));
        teamManagement.AddChild(easyshifthqPermissions.TeamManagement.BulkInvite, L("Permission:TeamManagement.BulkInvite"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<easyshifthqResource>(name);
    }
}

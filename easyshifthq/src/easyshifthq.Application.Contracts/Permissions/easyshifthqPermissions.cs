namespace easyshifthq.Permissions;

public static class easyshifthqPermissions
{
    public const string GroupName = "easyshifthq";

    public static class TeamManagement
    {
        public const string Default = GroupName + ".TeamManagement";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string BulkInvite = Default + ".BulkInvite";
    }
}

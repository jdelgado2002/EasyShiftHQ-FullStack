namespace easyshifthq.Permissions;

public static class LocationPermissions
{
    public const string GroupName = "Location";

    public static class Locations
    {
        public const string Default = GroupName + ".Default";
        public const string Create = GroupName + ".Create";
        public const string Edit = GroupName + ".Edit";
        public const string Delete = GroupName + ".Delete";
        public const string ManageActivity = GroupName + ".ManageActivity";
    }
}
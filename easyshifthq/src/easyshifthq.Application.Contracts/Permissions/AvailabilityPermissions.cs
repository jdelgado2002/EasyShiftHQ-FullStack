using Volo.Abp.Reflection;

namespace easyshifthq.Permissions;

public static class AvailabilityPermissions
{
    public const string GroupName = "Availability";

    public static class Availabilities
    {
        public const string Default = GroupName + ".Default";
        public const string Create = GroupName + ".Create";
        public const string Edit = GroupName + ".Edit";
        public const string Delete = GroupName + ".Delete";
        public const string Approve = GroupName + ".Approve";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AvailabilityPermissions));
    }
}

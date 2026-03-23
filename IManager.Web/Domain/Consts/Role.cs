namespace IManager.Web.Domain.Consts;

public static class Role
{
    public const string Staff = "Staff";
    public const string Admin = "Admin";
    public const string User = "User";

    public static readonly string[] All = { Staff, Admin, User };

    public static readonly string[] AdminAllowed = { Admin, User };
}
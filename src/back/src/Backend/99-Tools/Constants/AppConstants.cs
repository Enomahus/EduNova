namespace Tools.Constants;

public static class AppConstants
{
    public static readonly string SuperAdminRole = "SuperAdmin";
    public static readonly string StudentRole = "Student";
    public static readonly string ParentRole = "Parent";

    public const string ConfirmPasswordResetLink = "{0}/reset-password?token={1}&email={2}";
}

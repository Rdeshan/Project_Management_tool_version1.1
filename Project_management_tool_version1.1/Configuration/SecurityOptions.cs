namespace Project_management_tool_version1._1.Configuration;

public class SecurityOptions
{
    public const string SectionName = "Security";

    public int SessionTimeoutMinutes { get; set; } = 30;
    public int LockoutMaxFailedAttempts { get; set; } = 10;
    public int LockoutWindowMinutes { get; set; } = 15;
    public int LockoutDurationMinutes { get; set; } = 15;
    public int PasswordResetTokenMinutes { get; set; } = 30;
}

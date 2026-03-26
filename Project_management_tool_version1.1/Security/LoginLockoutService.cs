using System.Collections.Concurrent;
using Project_management_tool_version1._1.Configuration;

namespace Project_management_tool_version1._1.Security;

public class LoginLockoutService(SecurityOptions options)
{
    private readonly ConcurrentDictionary<string, AttemptState> attempts = new(StringComparer.OrdinalIgnoreCase);

    public bool IsLockedOut(string email, out TimeSpan remaining)
    {
        remaining = TimeSpan.Zero;
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        if (!attempts.TryGetValue(email, out var state) || state.LockedUntilUtc is null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        if (state.LockedUntilUtc <= now)
        {
            attempts.TryRemove(email, out _);
            return false;
        }

        remaining = state.LockedUntilUtc.Value - now;
        return true;
    }

    public void RecordFailure(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var now = DateTime.UtcNow;
        attempts.AddOrUpdate(
            email,
            _ => new AttemptState(1, now, null),
            (_, existing) =>
            {
                if (existing.LockedUntilUtc is not null && existing.LockedUntilUtc > now)
                {
                    return existing;
                }

                var windowMinutes = Math.Max(1, options.LockoutWindowMinutes);
                var failedAttempts = now - existing.FirstFailureUtc > TimeSpan.FromMinutes(windowMinutes)
                    ? 1
                    : existing.FailedAttempts + 1;

                var firstFailureUtc = failedAttempts == 1 ? now : existing.FirstFailureUtc;

                if (failedAttempts >= Math.Max(1, options.LockoutMaxFailedAttempts))
                {
                    var lockoutMinutes = Math.Max(1, options.LockoutDurationMinutes);
                    return new AttemptState(failedAttempts, firstFailureUtc, now.AddMinutes(lockoutMinutes));
                }

                return new AttemptState(failedAttempts, firstFailureUtc, null);
            });
    }

    public void ClearFailures(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        attempts.TryRemove(email, out _);
    }

    private sealed record AttemptState(int FailedAttempts, DateTime FirstFailureUtc, DateTime? LockedUntilUtc);
}

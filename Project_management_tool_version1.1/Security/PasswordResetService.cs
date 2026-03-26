using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Project_management_tool_version1._1.Configuration;

namespace Project_management_tool_version1._1.Security;

public class PasswordResetService(IMemoryCache memoryCache, SecurityOptions securityOptions)
{
    private static string BuildCacheKey(string normalizedEmail) => $"pwd-reset:{normalizedEmail}";

    public string CreateToken(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        var hash = ComputeHash(token);
        var lifetimeMinutes = Math.Max(1, securityOptions.PasswordResetTokenMinutes);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(lifetimeMinutes);

        memoryCache.Set(
            BuildCacheKey(normalizedEmail),
            new PasswordResetEntry(hash, expiresAtUtc),
            expiresAtUtc);

        return token;
    }

    public bool ValidateToken(string email, string token)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var cacheKey = BuildCacheKey(normalizedEmail);

        if (!memoryCache.TryGetValue(cacheKey, out PasswordResetEntry? entry) || entry is null)
        {
            return false;
        }

        if (entry.ExpiresAtUtc < DateTime.UtcNow)
        {
            memoryCache.Remove(cacheKey);
            return false;
        }

        var isValid = entry.TokenHash == ComputeHash(token);
        if (isValid)
        {
            memoryCache.Remove(cacheKey);
        }

        return isValid;
    }

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    private sealed record PasswordResetEntry(string TokenHash, DateTime ExpiresAtUtc);
}

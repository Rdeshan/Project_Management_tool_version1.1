using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;
using ProjectManagementTool.Infrastructure.Data;

namespace ProjectManagementTool.Infrastructure.Services;

public class UserService(ApplicationDbContext dbContext) : IUserService
{
    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User> CreateAsync(string firstName, string lastName, string email, string passwordHash, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var duplicateEmail = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == normalizedEmail, cancellationToken);

        if (duplicateEmail)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            Status = UserStatus.Active
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task UpdateAsync(
        Guid id,
        string firstName,
        string lastName,
        string email,
        UserStatus status,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("User was not found.");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var duplicateEmail = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == normalizedEmail && x.Id != id, cancellationToken);

        if (duplicateEmail)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        user.FirstName = firstName.Trim();
        user.LastName = lastName.Trim();
        user.Email = normalizedEmail;
        user.Status = status;
        user.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return;
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var candidate = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail && x.Status == UserStatus.Active, cancellationToken);

        if (candidate is null)
        {
            return null;
        }

        return candidate.PasswordHash == password ? candidate : null;
    }

    public async Task<User?> GetActiveByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var normalizedRoleName = roleName.Trim().ToLowerInvariant();

        return await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Status == UserStatus.Active)
            .FirstOrDefaultAsync(
                x => x.UserRoles.Any(ur => ur.Role != null && ur.Role.Name.ToLower() == normalizedRoleName),
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Role != null)
            .Select(x => x.Role!.Name)
            .Distinct()
            .OrderBy(x => x)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guid>> GetRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guid>> GetTeamIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserTeams
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.TeamId)
            .ToArrayAsync(cancellationToken);
    }

    public async Task UpdateRolesAsync(Guid userId, IReadOnlyCollection<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var userExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new InvalidOperationException("User was not found.");
        }

        var normalizedRoleIds = roleIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        var validRoleIds = await dbContext.Roles
            .AsNoTracking()
            .Where(x => normalizedRoleIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        if (normalizedRoleIds.Length != validRoleIds.Length)
        {
            throw new InvalidOperationException("One or more selected roles are invalid.");
        }

        var existing = await dbContext.UserRoles
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        dbContext.UserRoles.RemoveRange(existing.Where(x => !normalizedRoleIds.Contains(x.RoleId)));

        var existingRoleIds = existing.Select(x => x.RoleId).ToHashSet();
        var toAdd = normalizedRoleIds
            .Where(x => !existingRoleIds.Contains(x))
            .Select(roleId => new UserRole
            {
                UserId = userId,
                RoleId = roleId
            })
            .ToArray();

        dbContext.UserRoles.AddRange(toAdd);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTeamsAsync(Guid userId, IReadOnlyCollection<Guid> teamIds, CancellationToken cancellationToken = default)
    {
        var userExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new InvalidOperationException("User was not found.");
        }

        var normalizedTeamIds = teamIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        var validTeamIds = await dbContext.Teams
            .AsNoTracking()
            .Where(x => normalizedTeamIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        if (normalizedTeamIds.Length != validTeamIds.Length)
        {
            throw new InvalidOperationException("One or more selected teams are invalid.");
        }

        var existing = await dbContext.UserTeams
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        dbContext.UserTeams.RemoveRange(existing.Where(x => !normalizedTeamIds.Contains(x.TeamId)));

        var existingTeamIds = existing.Select(x => x.TeamId).ToHashSet();
        var toAdd = normalizedTeamIds
            .Where(x => !existingTeamIds.Contains(x))
            .Select(teamId => new UserTeam
            {
                UserId = userId,
                TeamId = teamId
            })
            .ToArray();

        dbContext.UserTeams.AddRange(toAdd);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>> GetRoleNamesByUserIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        if (userIds.Count == 0)
        {
            return new Dictionary<Guid, IReadOnlyCollection<string>>();
        }

        var pairs = await dbContext.UserRoles
            .AsNoTracking()
            .Where(x => userIds.Contains(x.UserId) && x.Role != null)
            .Select(x => new { x.UserId, RoleName = x.Role!.Name })
            .ToListAsync(cancellationToken);

        return pairs
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyCollection<string>)g
                    .Select(x => x.RoleName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray());
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>> GetTeamNamesByUserIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        if (userIds.Count == 0)
        {
            return new Dictionary<Guid, IReadOnlyCollection<string>>();
        }

        var pairs = await dbContext.UserTeams
            .AsNoTracking()
            .Where(x => userIds.Contains(x.UserId) && x.Team != null)
            .Select(x => new { x.UserId, TeamName = x.Team!.Name })
            .ToListAsync(cancellationToken);

        return pairs
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyCollection<string>)g
                    .Select(x => x.TeamName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray());
    }
}

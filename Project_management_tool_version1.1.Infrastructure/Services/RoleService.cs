using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;
using ProjectManagementTool.Infrastructure.Data;

namespace ProjectManagementTool.Infrastructure.Services;

public class RoleService(ApplicationDbContext dbContext) : IRoleService
{
    public async Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Role> CreateAsync(string name, string? description, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        var exists = await dbContext.Roles
            .AsNoTracking()
            .AnyAsync(x => x.Name == normalizedName, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Role name already exists.");
        }

        var role = new Role
        {
            Name = normalizedName,
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
        };

        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync(cancellationToken);

        return role;
    }

    public async Task UpdateAsync(Guid id, string name, string? description, CancellationToken cancellationToken = default)
    {
        var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (role is null)
        {
            throw new InvalidOperationException("Role was not found.");
        }

        var normalizedName = name.Trim();
        var duplicate = await dbContext.Roles
            .AsNoTracking()
            .AnyAsync(x => x.Name == normalizedName && x.Id != id, cancellationToken);

        if (duplicate)
        {
            throw new InvalidOperationException("Role name already exists.");
        }

        role.Name = normalizedName;
        role.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        role.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (role is null)
        {
            return;
        }

        dbContext.Roles.Remove(role);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

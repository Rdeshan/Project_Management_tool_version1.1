using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;
using ProjectManagementTool.Infrastructure.Data;

namespace ProjectManagementTool.Infrastructure.Services;

public class TeamService(ApplicationDbContext dbContext) : ITeamService
{
    public async Task<IReadOnlyCollection<Team>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Teams
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Team> CreateAsync(string name, string? description, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        var exists = await dbContext.Teams
            .AsNoTracking()
            .AnyAsync(x => x.Name == normalizedName, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Team name already exists.");
        }

        var team = new Team
        {
            Name = normalizedName,
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
        };

        dbContext.Teams.Add(team);
        await dbContext.SaveChangesAsync(cancellationToken);

        return team;
    }

    public async Task UpdateAsync(Guid id, string name, string? description, CancellationToken cancellationToken = default)
    {
        var team = await dbContext.Teams.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (team is null)
        {
            throw new InvalidOperationException("Team was not found.");
        }

        var normalizedName = name.Trim();
        var duplicate = await dbContext.Teams
            .AsNoTracking()
            .AnyAsync(x => x.Name == normalizedName && x.Id != id, cancellationToken);

        if (duplicate)
        {
            throw new InvalidOperationException("Team name already exists.");
        }

        team.Name = normalizedName;
        team.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        team.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var team = await dbContext.Teams.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (team is null)
        {
            return;
        }

        dbContext.Teams.Remove(team);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

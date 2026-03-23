using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;
using ProjectManagementTool.Infrastructure.Data;

namespace ProjectManagementTool.Infrastructure.Services;

public class ProjectService(ApplicationDbContext dbContext) : IProjectService
{
    public async Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Project> CreateAsync(
        string name,
        string code,
        string? description,
        string? clientName,
        DateOnly? startDate,
        DateOnly? expectedEndDate,
        ProjectStatus status,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var codeExists = await dbContext.Projects
            .AsNoTracking()
            .AnyAsync(x => x.Code == normalizedCode, cancellationToken);

        if (codeExists)
        {
            throw new InvalidOperationException("Project code already exists.");
        }

        var project = new Project
        {
            Name = name.Trim(),
            Code = normalizedCode,
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            ClientName = string.IsNullOrWhiteSpace(clientName) ? null : clientName.Trim(),
            StartDate = startDate,
            ExpectedEndDate = expectedEndDate,
            Status = status
        };

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task UpdateAsync(
        Guid id,
        string name,
        string code,
        string? description,
        string? clientName,
        DateOnly? startDate,
        DateOnly? expectedEndDate,
        ProjectStatus status,
        CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (project is null)
        {
            throw new InvalidOperationException("Project was not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var duplicateCode = await dbContext.Projects
            .AsNoTracking()
            .AnyAsync(x => x.Code == normalizedCode && x.Id != id, cancellationToken);

        if (duplicateCode)
        {
            throw new InvalidOperationException("Project code already exists.");
        }

        project.Name = name.Trim();
        project.Code = normalizedCode;
        project.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        project.ClientName = string.IsNullOrWhiteSpace(clientName) ? null : clientName.Trim();
        project.StartDate = startDate;
        project.ExpectedEndDate = expectedEndDate;
        project.Status = status;
        project.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (project is null)
        {
            return;
        }

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

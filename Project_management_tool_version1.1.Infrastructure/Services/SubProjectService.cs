using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;
using ProjectManagementTool.Infrastructure.Data;

namespace ProjectManagementTool.Infrastructure.Services;

public class SubProjectService(ApplicationDbContext dbContext) : ISubProjectService
{
    public async Task<IReadOnlyCollection<SubProject>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SubProjects
            .AsNoTracking()
            .Include(x => x.Product)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<SubProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.SubProjects
            .AsNoTracking()
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SubProject> CreateAsync(
        string name,
        string? description,
        string? moduleOwner,
        SubProjectStatus status,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var productExists = await dbContext.Products
            .AsNoTracking()
            .AnyAsync(x => x.Id == productId, cancellationToken);

        if (!productExists)
        {
            throw new InvalidOperationException("Selected product was not found.");
        }

        var subProject = new SubProject
        {
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            ModuleOwner = string.IsNullOrWhiteSpace(moduleOwner) ? null : moduleOwner.Trim(),
            Status = status,
            ProductId = productId
        };

        dbContext.SubProjects.Add(subProject);
        await dbContext.SaveChangesAsync(cancellationToken);

        return subProject;
    }

    public async Task UpdateAsync(
        Guid id,
        string name,
        string? description,
        string? moduleOwner,
        SubProjectStatus status,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var subProject = await dbContext.SubProjects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (subProject is null)
        {
            throw new InvalidOperationException("SubProject was not found.");
        }

        var productExists = await dbContext.Products
            .AsNoTracking()
            .AnyAsync(x => x.Id == productId, cancellationToken);

        if (!productExists)
        {
            throw new InvalidOperationException("Selected product was not found.");
        }

        subProject.Name = name.Trim();
        subProject.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        subProject.ModuleOwner = string.IsNullOrWhiteSpace(moduleOwner) ? null : moduleOwner.Trim();
        subProject.Status = status;
        subProject.ProductId = productId;
        subProject.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subProject = await dbContext.SubProjects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (subProject is null)
        {
            return;
        }

        dbContext.SubProjects.Remove(subProject);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

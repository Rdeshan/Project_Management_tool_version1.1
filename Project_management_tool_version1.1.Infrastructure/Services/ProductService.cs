using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;
using ProjectManagementTool.Infrastructure.Data;

namespace ProjectManagementTool.Infrastructure.Services;

public class ProductService(ApplicationDbContext dbContext) : IProductService
{
    public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Project)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Product> CreateAsync(
        string name,
        string versionName,
        string? description,
        DateOnly? plannedReleaseDate,
        ProductStatus status,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var projectExists = await dbContext.Projects
            .AsNoTracking()
            .AnyAsync(x => x.Id == projectId, cancellationToken);

        if (!projectExists)
        {
            throw new InvalidOperationException("Selected project was not found.");
        }

        var product = new Product
        {
            Name = name.Trim(),
            VersionName = versionName.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            PlannedReleaseDate = plannedReleaseDate,
            Status = status,
            ProjectId = projectId
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return product;
    }

    public async Task UpdateAsync(
        Guid id,
        string name,
        string versionName,
        string? description,
        DateOnly? plannedReleaseDate,
        ProductStatus status,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null)
        {
            throw new InvalidOperationException("Product was not found.");
        }

        var projectExists = await dbContext.Projects
            .AsNoTracking()
            .AnyAsync(x => x.Id == projectId, cancellationToken);

        if (!projectExists)
        {
            throw new InvalidOperationException("Selected project was not found.");
        }

        product.Name = name.Trim();
        product.VersionName = versionName.Trim();
        product.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        product.PlannedReleaseDate = plannedReleaseDate;
        product.Status = status;
        product.ProjectId = projectId;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null)
        {
            return;
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

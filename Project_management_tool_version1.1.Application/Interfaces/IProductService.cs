using ProjectManagementTool.Domain.Entities;

namespace ProjectManagementTool.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product> CreateAsync(
        string name,
        string versionName,
        string? description,
        DateOnly? plannedReleaseDate,
        ProductStatus status,
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(
        Guid id,
        string name,
        string versionName,
        string? description,
        DateOnly? plannedReleaseDate,
        ProductStatus status,
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

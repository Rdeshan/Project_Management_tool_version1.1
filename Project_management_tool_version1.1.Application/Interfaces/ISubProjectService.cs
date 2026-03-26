using ProjectManagementTool.Domain.Entities;

namespace ProjectManagementTool.Application.Interfaces;

public interface ISubProjectService
{
    Task<IReadOnlyCollection<SubProject>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SubProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SubProject> CreateAsync(
        string name,
        string? description,
        string? moduleOwner,
        SubProjectStatus status,
        Guid productId,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(
        Guid id,
        string name,
        string? description,
        string? moduleOwner,
        SubProjectStatus status,
        Guid productId,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

using ProjectManagementTool.Domain.Entities;

namespace ProjectManagementTool.Application.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project> CreateAsync(
        string name,
        string code,
        string? description,
        string? clientName,
        DateOnly? startDate,
        DateOnly? expectedEndDate,
        ProjectStatus status,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(
        Guid id,
        string name,
        string code,
        string? description,
        string? clientName,
        DateOnly? startDate,
        DateOnly? expectedEndDate,
        ProjectStatus status,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace ProjectManagementTool.Application.services;

public class ProjectService : IProjectService
{
    public Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Project> projects = Array.Empty<Project>();
        return Task.FromResult(projects);
    }

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Project?>(null);
    }

    public Task<Project> CreateAsync(
        string name,
        string code,
        string? description,
        string? clientName,
        DateOnly? startDate,
        DateOnly? expectedEndDate,
        ProjectStatus status,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Use infrastructure implementation for data operations.");
    }

    public Task UpdateAsync(
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
        throw new NotSupportedException("Use infrastructure implementation for data operations.");
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Use infrastructure implementation for data operations.");
    }
}

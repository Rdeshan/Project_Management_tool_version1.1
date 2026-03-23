using ProjectManagementTool.Domain.Entities;

namespace ProjectManagementTool.Application.Interfaces;

public interface ITeamService
{
    Task<IReadOnlyCollection<Team>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Team> CreateAsync(string name, string? description, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, string name, string? description, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

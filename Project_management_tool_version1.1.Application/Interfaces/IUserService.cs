using ProjectManagementTool.Domain.Entities;

namespace ProjectManagementTool.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(string firstName, string lastName, string email, string passwordHash, CancellationToken cancellationToken = default);
    Task UpdateAsync(
        Guid id,
        string firstName,
        string lastName,
        string email,
        UserStatus status,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<User?> GetActiveByRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Guid>> GetRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Guid>> GetTeamIdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateRolesAsync(Guid userId, IReadOnlyCollection<Guid> roleIds, CancellationToken cancellationToken = default);
    Task UpdateTeamsAsync(Guid userId, IReadOnlyCollection<Guid> teamIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>> GetRoleNamesByUserIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>> GetTeamNamesByUserIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken = default);
}

using ProjectManagementTool.Domain.Entities;

namespace ProjectManagementTool.Application.Interfaces;

public interface ITicketService
{
    Task<IReadOnlyCollection<Ticket>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Ticket> CreateAsync(
        string ticketNumber,
        string title,
        string description,
        TicketCategory category,
        TicketPriority priority,
        TicketStatus status,
        Guid projectId,
        Guid? productId,
        Guid? subProjectId,
        DateOnly? expectedDueDate,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(
        Guid id,
        string ticketNumber,
        string title,
        string description,
        TicketCategory category,
        TicketPriority priority,
        TicketStatus status,
        Guid projectId,
        Guid? productId,
        Guid? subProjectId,
        DateOnly? expectedDueDate,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

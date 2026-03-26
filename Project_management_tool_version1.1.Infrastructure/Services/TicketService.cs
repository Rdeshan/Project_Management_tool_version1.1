using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;
using ProjectManagementTool.Infrastructure.Data;

namespace ProjectManagementTool.Infrastructure.Services;

public class TicketService(ApplicationDbContext dbContext) : ITicketService
{
    public async Task<IReadOnlyCollection<Ticket>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.Product)
            .Include(x => x.SubProject)
            .OrderBy(x => x.TicketNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.Product)
            .Include(x => x.SubProject)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Ticket> CreateAsync(
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
        CancellationToken cancellationToken = default)
    {
        var normalizedTicketNumber = ticketNumber.Trim().ToUpperInvariant();
        var duplicateTicketNumber = await dbContext.Tickets
            .AsNoTracking()
            .AnyAsync(x => x.TicketNumber == normalizedTicketNumber, cancellationToken);

        if (duplicateTicketNumber)
        {
            throw new InvalidOperationException("Ticket number already exists.");
        }

        await ValidateReferencesAsync(projectId, productId, subProjectId, cancellationToken);

        var ticket = new Ticket
        {
            TicketNumber = normalizedTicketNumber,
            Title = title.Trim(),
            Description = description.Trim(),
            Category = category,
            Priority = priority,
            Status = status,
            ProjectId = projectId,
            ProductId = productId,
            SubProjectId = subProjectId,
            ExpectedDueDate = expectedDueDate
        };

        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ticket;
    }

    public async Task UpdateAsync(
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
        CancellationToken cancellationToken = default)
    {
        var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (ticket is null)
        {
            throw new InvalidOperationException("Ticket was not found.");
        }

        var normalizedTicketNumber = ticketNumber.Trim().ToUpperInvariant();
        var duplicateTicketNumber = await dbContext.Tickets
            .AsNoTracking()
            .AnyAsync(x => x.TicketNumber == normalizedTicketNumber && x.Id != id, cancellationToken);

        if (duplicateTicketNumber)
        {
            throw new InvalidOperationException("Ticket number already exists.");
        }

        await ValidateReferencesAsync(projectId, productId, subProjectId, cancellationToken);

        ticket.TicketNumber = normalizedTicketNumber;
        ticket.Title = title.Trim();
        ticket.Description = description.Trim();
        ticket.Category = category;
        ticket.Priority = priority;
        ticket.Status = status;
        ticket.ProjectId = projectId;
        ticket.ProductId = productId;
        ticket.SubProjectId = subProjectId;
        ticket.ExpectedDueDate = expectedDueDate;
        ticket.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (ticket is null)
        {
            return;
        }

        dbContext.Tickets.Remove(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateReferencesAsync(
        Guid projectId,
        Guid? productId,
        Guid? subProjectId,
        CancellationToken cancellationToken)
    {
        var projectExists = await dbContext.Projects
            .AsNoTracking()
            .AnyAsync(x => x.Id == projectId, cancellationToken);

        if (!projectExists)
        {
            throw new InvalidOperationException("Selected project was not found.");
        }

        if (productId.HasValue)
        {
            var productExists = await dbContext.Products
                .AsNoTracking()
                .AnyAsync(x => x.Id == productId.Value, cancellationToken);

            if (!productExists)
            {
                throw new InvalidOperationException("Selected product was not found.");
            }
        }

        if (subProjectId.HasValue)
        {
            var subProjectExists = await dbContext.SubProjects
                .AsNoTracking()
                .AnyAsync(x => x.Id == subProjectId.Value, cancellationToken);

            if (!subProjectExists)
            {
                throw new InvalidOperationException("Selected subproject was not found.");
            }
        }
    }
}

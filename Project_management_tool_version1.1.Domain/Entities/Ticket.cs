namespace ProjectManagementTool.Domain.Entities;

public class Ticket : BaseEntity
{
    public string TicketNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public TicketCategory Category { get; set; } = TicketCategory.Task;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpectedDueDate { get; set; }
    public DateOnly? RevisedDueDate { get; set; }
    public DateOnly? ActualEndDate { get; set; }
    public string? DelayReason { get; set; }

    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }

    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }

    public Guid? SubProjectId { get; set; }
    public SubProject? SubProject { get; set; }
}

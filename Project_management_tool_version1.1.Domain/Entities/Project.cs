namespace ProjectManagementTool.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ClientName { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpectedEndDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

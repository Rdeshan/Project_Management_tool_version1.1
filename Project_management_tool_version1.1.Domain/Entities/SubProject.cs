namespace ProjectManagementTool.Domain.Entities;

public class SubProject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ModuleOwner { get; set; }
    public SubProjectStatus Status { get; set; } = SubProjectStatus.NotStarted;

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

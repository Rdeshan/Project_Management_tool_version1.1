namespace ProjectManagementTool.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string VersionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? PlannedReleaseDate { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Planned;

    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<SubProject> SubProjects { get; set; } = new List<SubProject>();
}

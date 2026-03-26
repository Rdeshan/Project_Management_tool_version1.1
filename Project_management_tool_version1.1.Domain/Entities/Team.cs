namespace ProjectManagementTool.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
}

using ProjectManagementTool.Domain.Entities;

namespace ProjectManagementTool.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext dbContext)
    {
        if (!dbContext.Projects.Any())
        {
            var project = new Project
            {
                Name = "University Management System",
                Code = "UMS",
                Description = "Initial seeded project",
                ClientName = "Demo Client",
                Status = ProjectStatus.Active,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var product = new Product
            {
                Name = "UMS Release",
                VersionName = "v1.0",
                Description = "Initial release",
                PlannedReleaseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2)),
                Status = ProductStatus.InDevelopment,
                Project = project
            };

            var subProject = new SubProject
            {
                Name = "Student Registration Portal",
                Description = "Core student onboarding module",
                ModuleOwner = "Development Team",
                Status = SubProjectStatus.InProgress,
                Product = product
            };

            var ticket = new Ticket
            {
                TicketNumber = "UMS-001",
                Title = "Set up project foundation",
                Description = "Create initial entities and DbContext.",
                Category = TicketCategory.Task,
                Priority = TicketPriority.High,
                Status = TicketStatus.Open,
                Project = project,
                Product = product,
                SubProject = subProject,
                ExpectedDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
            };

            dbContext.Projects.Add(project);
            dbContext.Products.Add(product);
            dbContext.SubProjects.Add(subProject);
            dbContext.Tickets.Add(ticket);
        }

        var coreTeam = dbContext.Teams.FirstOrDefault(x => x.Name == "Core Team");
        if (coreTeam is null)
        {
            coreTeam = new Team
            {
                Name = "Core Team",
                Description = "Initial default team"
            };

            dbContext.Teams.Add(coreTeam);
        }

        var developmentSeedUsers = new[]
        {
            new { RoleName = "Admin", RoleDescription = "System administrator", FirstName = "System", LastName = "Admin", Email = "admin@projecttool.local", Password = "Admin@123" },
            new { RoleName = "ProjectManager", RoleDescription = "Project planning and execution", FirstName = "Priya", LastName = "Manager", Email = "pm@projecttool.local", Password = "ProjectManager@123" },
            new { RoleName = "Developer", RoleDescription = "Builds product features", FirstName = "Dev", LastName = "Engineer", Email = "dev@projecttool.local", Password = "Developer@123" },
            new { RoleName = "QA", RoleDescription = "Quality assurance", FirstName = "Quality", LastName = "Tester", Email = "qa@projecttool.local", Password = "Qa@123" },
            new { RoleName = "BusinessAnalyst", RoleDescription = "Manages requirements and backlog", FirstName = "Business", LastName = "Analyst", Email = "ba@projecttool.local", Password = "BusinessAnalyst@123" },
            new { RoleName = "Viewer", RoleDescription = "Read-only project access", FirstName = "Read", LastName = "Only", Email = "viewer@projecttool.local", Password = "Viewer@123" }
        };

        foreach (var item in developmentSeedUsers)
        {
            var role = dbContext.Roles.FirstOrDefault(x => x.Name == item.RoleName);
            if (role is null)
            {
                role = new Role
                {
                    Name = item.RoleName,
                    Description = item.RoleDescription
                };

                dbContext.Roles.Add(role);
            }

            var user = dbContext.Users.FirstOrDefault(x => x.Email == item.Email);
            if (user is null)
            {
                user = new User
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    PasswordHash = item.Password,
                    Status = UserStatus.Active
                };

                dbContext.Users.Add(user);
            }

            var hasRole = dbContext.UserRoles.Any(x => x.UserId == user.Id && x.RoleId == role.Id);
            if (!hasRole)
            {
                dbContext.UserRoles.Add(new UserRole
                {
                    User = user,
                    Role = role
                });
            }

            var hasTeam = dbContext.UserTeams.Any(x => x.UserId == user.Id && x.TeamId == coreTeam.Id);
            if (!hasTeam)
            {
                dbContext.UserTeams.Add(new UserTeam
                {
                    User = user,
                    Team = coreTeam
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }
}

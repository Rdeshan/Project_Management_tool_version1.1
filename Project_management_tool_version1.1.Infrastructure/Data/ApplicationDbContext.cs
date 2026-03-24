using Microsoft.EntityFrameworkCore;
using ProjectManagementTool.Domain.Entities;

namespace ProjectManagementTool.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserTeam> UserTeams => Set<UserTeam>();

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SubProject> SubProjects => Set<SubProject>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();

            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(300);

            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(300);

            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.RoleId });

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserTeam>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.TeamId });

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserTeams)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Team)
                .WithMany(x => x.UserTeams)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.VersionName).HasMaxLength(50).IsRequired();

            entity.HasOne(x => x.Project)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubProject>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ModuleOwner).HasMaxLength(120);

            entity.HasOne(x => x.Product)
                .WithMany(x => x.SubProjects)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(x => x.TicketNumber).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.Property(x => x.DelayReason).HasMaxLength(500);

            entity.HasIndex(x => x.TicketNumber).IsUnique();
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.ExpectedDueDate);

            entity.HasOne(x => x.Project)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.SubProject)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.SubProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

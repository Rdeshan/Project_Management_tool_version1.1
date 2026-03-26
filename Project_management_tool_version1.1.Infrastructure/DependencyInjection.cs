using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Infrastructure.Data;
using ProjectManagementTool.Infrastructure.Services;

namespace ProjectManagementTool.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)));

        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISubProjectService, SubProjectService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITeamService, TeamService>();

        return services;
    }
}

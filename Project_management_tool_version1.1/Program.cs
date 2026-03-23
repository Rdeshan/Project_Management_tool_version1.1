using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjectManagementTool.Infrastructure;
using ProjectManagementTool.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EditorAccess", policy => policy.RequireRole("Admin", "ProjectManager", "Developer", "QA"));
    options.AddPolicy("NotificationManager", policy => policy.RequireRole("Admin", "ProjectManager"));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Privacy");
    options.Conventions.AllowAnonymousToPage("/Auth/Login");
    options.Conventions.AllowAnonymousToPage("/AccessDenied");

    options.Conventions.AuthorizeFolder("/Users", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Roles", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Teams", "AdminOnly");
    options.Conventions.AuthorizePage("/Notifications/Settings", "NotificationManager");

    options.Conventions.AuthorizePage("/Projects/Create", "EditorAccess");
    options.Conventions.AuthorizePage("/Projects/Edit", "EditorAccess");
    options.Conventions.AuthorizePage("/Projects/Delete", "EditorAccess");

    options.Conventions.AuthorizePage("/Products/Create", "EditorAccess");
    options.Conventions.AuthorizePage("/Products/Edit", "EditorAccess");
    options.Conventions.AuthorizePage("/Products/Delete", "EditorAccess");

    options.Conventions.AuthorizePage("/SubProjects/Create", "EditorAccess");
    options.Conventions.AuthorizePage("/SubProjects/Edit", "EditorAccess");
    options.Conventions.AuthorizePage("/SubProjects/Delete", "EditorAccess");

    options.Conventions.AuthorizePage("/Tickets/Create", "EditorAccess");
    options.Conventions.AuthorizePage("/Tickets/Edit", "EditorAccess");
    options.Conventions.AuthorizePage("/Tickets/Delete", "EditorAccess");
});
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    await SeedData.InitializeAsync(dbContext);
}

app.Run();

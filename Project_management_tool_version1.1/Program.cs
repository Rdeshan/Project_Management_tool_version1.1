using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjectManagementTool.Infrastructure;
using ProjectManagementTool.Infrastructure.Data;
using Project_management_tool_version1._1.Configuration;
using Project_management_tool_version1._1.Security;
using Project_management_tool_version1._1.Services;

var builder = WebApplication.CreateBuilder(args);

var securityOptions = builder.Configuration
    .GetSection(SecurityOptions.SectionName)
    .Get<SecurityOptions>() ?? new SecurityOptions();
builder.Services.AddSingleton(securityOptions);
builder.Services.AddSingleton<LoginLockoutService>();
builder.Services.AddSingleton<PasswordResetService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IEmailSender, DevelopmentEmailSender>();

// Add services to the container.
var authenticationBuilder = builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(Math.Max(1, securityOptions.SessionTimeoutMinutes));
        options.SlidingExpiration = true;
    })
    .AddCookie("External");

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authenticationBuilder.AddGoogle("Google", options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.SignInScheme = "External";
    });
}

var microsoftClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
var microsoftClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];

if (!string.IsNullOrWhiteSpace(microsoftClientId) && !string.IsNullOrWhiteSpace(microsoftClientSecret))
{
    authenticationBuilder.AddMicrosoftAccount("Microsoft", options =>
    {
        options.ClientId = microsoftClientId;
        options.ClientSecret = microsoftClientSecret;
        options.SignInScheme = "External";
    });
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EditorAccess", policy => policy.RequireRole("Admin", "ProjectManager", "Developer", "QA", "BusinessAnalyst"));
    options.AddPolicy("NotificationManager", policy => policy.RequireRole("Admin", "ProjectManager"));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Privacy");
    options.Conventions.AllowAnonymousToPage("/Auth/Login");
    options.Conventions.AllowAnonymousToPage("/Auth/ForgotPassword");
    options.Conventions.AllowAnonymousToPage("/Auth/ResetPassword");
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
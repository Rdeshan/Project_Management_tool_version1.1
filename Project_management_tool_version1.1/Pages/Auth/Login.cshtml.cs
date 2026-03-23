using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;

namespace Project_management_tool_version1._1.Pages.Auth;

public class LoginModel(IUserService userService, IRoleService roleService, IWebHostEnvironment environment) : PageModel
{
    public IReadOnlyCollection<string> QuickRoles { get; private set; } = [];

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public bool IsAuthenticated { get; private set; }
    public string? AuthenticatedMessage { get; private set; }
    public bool IsDevelopment => environment.IsDevelopment();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public class LoginInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadQuickRolesAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadQuickRolesAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userService.ValidateCredentialsAsync(Input.Email, Input.Password, cancellationToken);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return Page();
        }

        await SignInUserAsync(user, cancellationToken);
        IsAuthenticated = true;
        AuthenticatedMessage = $"Login successful. Welcome {user.FirstName} {user.LastName}.";
        return LocalRedirect(string.IsNullOrWhiteSpace(ReturnUrl) ? "/" : ReturnUrl);
    }

    public async Task<IActionResult> OnPostQuickLoginAsync(string roleName, CancellationToken cancellationToken)
    {
        await LoadQuickRolesAsync(cancellationToken);

        if (!IsDevelopment)
        {
            return NotFound();
        }

        var user = await userService.GetActiveByRoleAsync(roleName, cancellationToken);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, $"No active user found for role '{roleName}'.");
            return Page();
        }

        await SignInUserAsync(user, cancellationToken);
        IsAuthenticated = true;
        AuthenticatedMessage = $"Quick login successful as {user.FirstName} {user.LastName} ({roleName}).";
        return LocalRedirect(string.IsNullOrWhiteSpace(ReturnUrl) ? "/" : ReturnUrl);
    }

    private async Task LoadQuickRolesAsync(CancellationToken cancellationToken)
    {
        if (!IsDevelopment)
        {
            QuickRoles = [];
            return;
        }

        QuickRoles = (await roleService.GetAllAsync(cancellationToken))
            .Select(x => x.Name)
            .OrderBy(x => x)
            .ToArray();
    }

    private async Task SignInUserAsync(ProjectManagementTool.Domain.Entities.User user, CancellationToken cancellationToken)
    {
        var roleNames = await userService.GetRoleNamesAsync(user.Id, cancellationToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(roleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}

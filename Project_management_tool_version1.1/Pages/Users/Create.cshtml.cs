using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Users;

public class CreateModel(IUserService userService, IRoleService roleService, ITeamService teamService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyCollection<SelectListItem> Statuses { get; } = Enum
        .GetValues<UserStatus>()
        .Select(x => new SelectListItem(x.ToString(), x.ToString()))
        .ToArray();

    public IReadOnlyCollection<SelectListItem> Roles { get; private set; } = [];
    public IReadOnlyCollection<SelectListItem> Teams { get; private set; } = [];

    public class InputModel
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserStatus Status { get; set; } = UserStatus.Active;

        public IReadOnlyCollection<Guid> RoleIds { get; set; } = [];
        public IReadOnlyCollection<Guid> TeamIds { get; set; } = [];
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadLookupsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadLookupsAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var user = await userService.CreateAsync(
                Input.FirstName,
                Input.LastName,
                Input.Email,
                Input.Password,
                cancellationToken);

            if (Input.Status != UserStatus.Active)
            {
                await userService.UpdateAsync(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    Input.Status,
                    cancellationToken);
            }

            await userService.UpdateRolesAsync(user.Id, Input.RoleIds, cancellationToken);
            await userService.UpdateTeamsAsync(user.Id, Input.TeamIds, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("/Users/Index");
    }

    private async Task LoadLookupsAsync(CancellationToken cancellationToken)
    {
        Roles = (await roleService.GetAllAsync(cancellationToken))
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToArray();

        Teams = (await teamService.GetAllAsync(cancellationToken))
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToArray();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Teams;

public class DeleteModel(ITeamService teamService) : PageModel
{
    public Team? Team { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Team = await teamService.GetByIdAsync(id, cancellationToken);
        if (Team is null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        await teamService.DeleteAsync(id, cancellationToken);
        return RedirectToPage("/Teams/Index");
    }
}

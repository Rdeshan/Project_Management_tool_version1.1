using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Projects;

public class DeleteModel(IProjectService projectService) : PageModel
{
    public Project? Project { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Project = await projectService.GetByIdAsync(id, cancellationToken);
        if (Project is null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        await projectService.DeleteAsync(id, cancellationToken);
        return RedirectToPage("/Projects/Index");
    }
}

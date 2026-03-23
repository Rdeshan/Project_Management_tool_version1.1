using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.SubProjects;

public class DeleteModel(ISubProjectService subProjectService) : PageModel
{
    public SubProject? SubProject { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        SubProject = await subProjectService.GetByIdAsync(id, cancellationToken);
        if (SubProject is null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        await subProjectService.DeleteAsync(id, cancellationToken);
        return RedirectToPage("/SubProjects/Index");
    }
}

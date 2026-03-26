using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Tickets;

public class DeleteModel(ITicketService ticketService) : PageModel
{
    public Ticket? Ticket { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Ticket = await ticketService.GetByIdAsync(id, cancellationToken);
        if (Ticket is null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        await ticketService.DeleteAsync(id, cancellationToken);
        return RedirectToPage("/Tickets/Index");
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages
{
    public class IndexModel(
        IProjectService projectService,
        IProductService productService,
        ISubProjectService subProjectService,
        ITicketService ticketService) : PageModel
    {
        public int ProjectCount { get; private set; }
        public int ProductCount { get; private set; }
        public int SubProjectCount { get; private set; }
        public int TicketCount { get; private set; }
        public int OpenTicketCount { get; private set; }
        public int ActiveTicketCount { get; private set; }
        public int CompletedTicketCount { get; private set; }
        public int OverdueTicketCount { get; private set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            try
            {
                ProjectCount = (await projectService.GetAllAsync(cancellationToken)).Count;
                ProductCount = (await productService.GetAllAsync(cancellationToken)).Count;
                SubProjectCount = (await subProjectService.GetAllAsync(cancellationToken)).Count;

                var tickets = await ticketService.GetAllAsync(cancellationToken);
                TicketCount = tickets.Count;

                OpenTicketCount = tickets.Count(x => x.Status == TicketStatus.Open || x.Status == TicketStatus.NotStarted);
                ActiveTicketCount = tickets.Count(x => x.Status is TicketStatus.Implementing or TicketStatus.InReview or TicketStatus.Qa or TicketStatus.Uat or TicketStatus.Reopened);
                CompletedTicketCount = tickets.Count(x => x.Status is TicketStatus.Completed or TicketStatus.Closed);

                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                OverdueTicketCount = tickets.Count(x =>
                    x.ExpectedDueDate.HasValue &&
                    x.ExpectedDueDate.Value < today &&
                    x.Status is not TicketStatus.Completed and not TicketStatus.Closed and not TicketStatus.Cancelled);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                ProjectCount = 0;
                ProductCount = 0;
                SubProjectCount = 0;
                TicketCount = 0;
                OpenTicketCount = 0;
                ActiveTicketCount = 0;
                CompletedTicketCount = 0;
                OverdueTicketCount = 0;
            }
        }
    }
}

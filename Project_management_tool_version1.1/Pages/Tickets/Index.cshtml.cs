using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Tickets;

public class IndexModel(ITicketService ticketService) : PageModel
{
    public IReadOnlyCollection<Ticket> Tickets { get; private set; } = [];
    public IReadOnlyCollection<string> AvailableStatuses { get; private set; } = [];
    public IReadOnlyCollection<string> AvailablePriorities { get; private set; } = [];
    public IReadOnlyCollection<string> AvailableCategories { get; private set; } = [];
    public IReadOnlyCollection<string> AvailableProjects { get; private set; } = [];
    public IReadOnlyCollection<int> PageSizeOptions { get; } = [10, 25, 50];

    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Priority { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Project { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "TicketNumber";

    [BindProperty(SupportsGet = true)]
    public string SortDirection { get; set; } = "asc";

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var allTickets = await ticketService.GetAllAsync(cancellationToken);

            AvailableStatuses = Enum.GetNames<TicketStatus>().OrderBy(x => x).ToArray();
            AvailablePriorities = Enum.GetNames<TicketPriority>().OrderBy(x => x).ToArray();
            AvailableCategories = Enum.GetNames<TicketCategory>().OrderBy(x => x).ToArray();
            AvailableProjects = allTickets
                .Select(x => x.Project?.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToArray();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var normalizedSearch = Search.Trim();
                allTickets = allTickets
                    .Where(x =>
                        x.TicketNumber.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        x.Title.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        (x.Project?.Name?.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Status) && Enum.TryParse<TicketStatus>(Status, out var selectedStatus))
            {
                allTickets = allTickets.Where(x => x.Status == selectedStatus).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Priority) && Enum.TryParse<TicketPriority>(Priority, out var selectedPriority))
            {
                allTickets = allTickets.Where(x => x.Priority == selectedPriority).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Category) && Enum.TryParse<TicketCategory>(Category, out var selectedCategory))
            {
                allTickets = allTickets.Where(x => x.Category == selectedCategory).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Project))
            {
                allTickets = allTickets
                    .Where(x => string.Equals(x.Project?.Name, Project, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }

            if (!PageSizeOptions.Contains(PageSize))
            {
                PageSize = 10;
            }

            var sorted = (SortBy, SortDirection.ToLowerInvariant()) switch
            {
                ("Title", "desc") => allTickets.OrderByDescending(x => x.Title),
                ("Title", _) => allTickets.OrderBy(x => x.Title),
                ("Project", "desc") => allTickets.OrderByDescending(x => x.Project!.Name),
                ("Project", _) => allTickets.OrderBy(x => x.Project!.Name),
                ("Category", "desc") => allTickets.OrderByDescending(x => x.Category).ThenBy(x => x.TicketNumber),
                ("Category", _) => allTickets.OrderBy(x => x.Category).ThenBy(x => x.TicketNumber),
                ("Priority", "desc") => allTickets.OrderByDescending(x => x.Priority).ThenBy(x => x.TicketNumber),
                ("Priority", _) => allTickets.OrderBy(x => x.Priority).ThenBy(x => x.TicketNumber),
                ("Status", "desc") => allTickets.OrderByDescending(x => x.Status).ThenBy(x => x.TicketNumber),
                ("Status", _) => allTickets.OrderBy(x => x.Status).ThenBy(x => x.TicketNumber),
                ("DueDate", "desc") => allTickets.OrderByDescending(x => x.ExpectedDueDate).ThenBy(x => x.TicketNumber),
                ("DueDate", _) => allTickets.OrderBy(x => x.ExpectedDueDate).ThenBy(x => x.TicketNumber),
                ("TicketNumber", "desc") => allTickets.OrderByDescending(x => x.TicketNumber),
                _ => allTickets.OrderBy(x => x.TicketNumber)
            };

            var sortedArray = sorted.ToArray();
            TotalCount = sortedArray.Length;
            TotalPages = Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

            if (Page < 1)
            {
                Page = 1;
            }

            if (Page > TotalPages)
            {
                Page = TotalPages;
            }

            Tickets = sortedArray
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToArray();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Tickets = [];
            AvailableStatuses = [];
            AvailablePriorities = [];
            AvailableCategories = [];
            AvailableProjects = [];
            TotalCount = 0;
            TotalPages = 0;
        }
    }
}

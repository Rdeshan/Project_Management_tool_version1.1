using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;
using System.Text;

namespace Project_management_tool_version1._1.Pages.Delays;

public class IndexModel(ITicketService ticketService) : PageModel
{
    public IReadOnlyCollection<DelayedTicketItem> DelayedTickets { get; private set; } = [];
    public IReadOnlyCollection<string> DelayTypes { get; } = ["Overdue Ticket", "Not Started - Late", "Paused - Extended"];
    public IReadOnlyCollection<int> PageSizeOptions { get; } = [10, 25, 50];

    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    public int TotalDelayedCount { get; private set; }
    public int OverdueCount { get; private set; }
    public int NotStartedLateCount { get; private set; }
    public int PausedExtendedCount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? DelayType { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var delayed = await GetDelayedItemsAsync(cancellationToken);

            TotalDelayedCount = delayed.Count;
            OverdueCount = delayed.Count(x => x.DelayType == "Overdue Ticket");
            NotStartedLateCount = delayed.Count(x => x.DelayType == "Not Started - Late");
            PausedExtendedCount = delayed.Count(x => x.DelayType == "Paused - Extended");

            var query = ApplyFilters(delayed);

            if (!PageSizeOptions.Contains(PageSize))
            {
                PageSize = 10;
            }

            var sorted = query
                .OrderByDescending(x => x.DaysDelayed)
                .ThenBy(x => x.Ticket.ExpectedDueDate)
                .ThenBy(x => x.Ticket.TicketNumber)
                .ToArray();

            TotalCount = sorted.Length;
            TotalPages = Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

            if (Page < 1)
            {
                Page = 1;
            }

            if (Page > TotalPages)
            {
                Page = TotalPages;
            }

            DelayedTickets = sorted
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToArray();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            DelayedTickets = [];
            TotalCount = 0;
            TotalPages = 0;
            TotalDelayedCount = 0;
            OverdueCount = 0;
            NotStartedLateCount = 0;
            PausedExtendedCount = 0;
        }
    }

    public async Task<IActionResult> OnGetExportCsvAsync(CancellationToken cancellationToken)
    {
        try
        {
            var delayed = await GetDelayedItemsAsync(cancellationToken);
            var filtered = ApplyFilters(delayed)
                .OrderByDescending(x => x.DaysDelayed)
                .ThenBy(x => x.Ticket.ExpectedDueDate)
                .ThenBy(x => x.Ticket.TicketNumber)
                .ToArray();

            var csv = new StringBuilder();
            csv.AppendLine("Ticket ID,Title,Project,SubProject,Assignee,Original Due Date,Revised Due Date,Delay Type,Days Delayed,Delay Reason");

            foreach (var item in filtered)
            {
                csv.AppendLine(string.Join(',',
                    EscapeCsv(item.Ticket.TicketNumber),
                    EscapeCsv(item.Ticket.Title),
                    EscapeCsv(item.Ticket.Project?.Name),
                    EscapeCsv(item.Ticket.SubProject?.Name),
                    EscapeCsv("N/A"),
                    EscapeCsv(item.Ticket.ExpectedDueDate?.ToString("yyyy-MM-dd")),
                    EscapeCsv(item.Ticket.RevisedDueDate?.ToString("yyyy-MM-dd")),
                    EscapeCsv(item.DelayType),
                    EscapeCsv(item.DaysDelayed.ToString()),
                    EscapeCsv(item.Ticket.DelayReason)));
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"delay-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";
            return File(bytes, "text/csv", fileName);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return new EmptyResult();
        }
    }

    private async Task<IReadOnlyCollection<DelayedTicketItem>> GetDelayedItemsAsync(CancellationToken cancellationToken)
    {
        var tickets = await ticketService.GetAllAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return tickets
            .Select(x => BuildDelayedItem(x, today))
            .Where(x => x is not null)
            .Cast<DelayedTicketItem>()
            .ToArray();
    }

    private IEnumerable<DelayedTicketItem> ApplyFilters(IEnumerable<DelayedTicketItem> source)
    {
        var query = source;

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var normalizedSearch = Search.Trim();
            query = query.Where(x =>
                x.Ticket.TicketNumber.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                x.Ticket.Title.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                (x.Ticket.Project?.Name?.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (!string.IsNullOrWhiteSpace(DelayType))
        {
            query = query.Where(x => string.Equals(x.DelayType, DelayType, StringComparison.OrdinalIgnoreCase));
        }

        if (FromDate.HasValue)
        {
            query = query.Where(x => x.DelayStartedOn >= FromDate.Value);
        }

        if (ToDate.HasValue)
        {
            query = query.Where(x => x.DelayStartedOn <= ToDate.Value);
        }

        return query;
    }

    private static DelayedTicketItem? BuildDelayedItem(Ticket ticket, DateOnly today)
    {
        if (ticket.ExpectedDueDate.HasValue &&
            ticket.ExpectedDueDate.Value < today &&
            ticket.Status is not TicketStatus.Completed and not TicketStatus.Closed and not TicketStatus.Cancelled)
        {
            return new DelayedTicketItem(
                ticket,
                "Overdue Ticket",
                today.DayNumber - ticket.ExpectedDueDate.Value.DayNumber,
                ticket.ExpectedDueDate.Value);
        }

        if (ticket.StartDate.HasValue &&
            ticket.StartDate.Value < today &&
            ticket.Status is TicketStatus.Open or TicketStatus.NotStarted)
        {
            return new DelayedTicketItem(
                ticket,
                "Not Started - Late",
                today.DayNumber - ticket.StartDate.Value.DayNumber,
                ticket.StartDate.Value);
        }

        var pausedSince = DateOnly.FromDateTime(ticket.UpdatedAtUtc ?? ticket.CreatedAtUtc);
        if (ticket.Status == TicketStatus.Paused && pausedSince.AddDays(3) < today)
        {
            return new DelayedTicketItem(
                ticket,
                "Paused - Extended",
                today.DayNumber - pausedSince.DayNumber,
                pausedSince);
        }

        return null;
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }

    public sealed record DelayedTicketItem(Ticket Ticket, string DelayType, int DaysDelayed, DateOnly DelayStartedOn);
}

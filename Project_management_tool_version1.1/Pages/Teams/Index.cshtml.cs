using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Teams;

public class IndexModel(ITeamService teamService) : PageModel
{
    public IReadOnlyCollection<Team> Teams { get; private set; } = [];
    public IReadOnlyCollection<int> PageSizeOptions { get; } = [10, 25, 50];

    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "Name";

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
            var allTeams = await teamService.GetAllAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var normalizedSearch = Search.Trim();
                allTeams = allTeams
                    .Where(x =>
                        x.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        (x.Description?.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToArray();
            }

            if (!PageSizeOptions.Contains(PageSize))
            {
                PageSize = 10;
            }

            var sorted = (SortBy, SortDirection.ToLowerInvariant()) switch
            {
                ("Description", "desc") => allTeams.OrderByDescending(x => x.Description).ThenBy(x => x.Name),
                ("Description", _) => allTeams.OrderBy(x => x.Description).ThenBy(x => x.Name),
                ("Name", "desc") => allTeams.OrderByDescending(x => x.Name),
                _ => allTeams.OrderBy(x => x.Name)
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

            Teams = sortedArray
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToArray();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Teams = [];
            TotalCount = 0;
            TotalPages = 0;
        }
    }
}

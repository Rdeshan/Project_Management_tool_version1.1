using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.SubProjects;

public class IndexModel(ISubProjectService subProjectService) : PageModel
{
    public IReadOnlyCollection<SubProject> SubProjects { get; private set; } = [];
    public IReadOnlyCollection<string> AvailableStatuses { get; private set; } = [];
    public IReadOnlyCollection<string> AvailableProducts { get; private set; } = [];
    public IReadOnlyCollection<int> PageSizeOptions { get; } = [10, 25, 50];

    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Product { get; set; }

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
            var allSubProjects = await subProjectService.GetAllAsync(cancellationToken);

            AvailableStatuses = Enum.GetNames<SubProjectStatus>().OrderBy(x => x).ToArray();
            AvailableProducts = allSubProjects
                .Select(x => x.Product?.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToArray();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var normalizedSearch = Search.Trim();
                allSubProjects = allSubProjects
                    .Where(x =>
                        x.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        (x.ModuleOwner?.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (x.Product?.Name?.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Status) && Enum.TryParse<SubProjectStatus>(Status, out var selectedStatus))
            {
                allSubProjects = allSubProjects.Where(x => x.Status == selectedStatus).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Product))
            {
                allSubProjects = allSubProjects
                    .Where(x => string.Equals(x.Product?.Name, Product, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }

            if (!PageSizeOptions.Contains(PageSize))
            {
                PageSize = 10;
            }

            var sorted = (SortBy, SortDirection.ToLowerInvariant()) switch
            {
                ("Product", "desc") => allSubProjects.OrderByDescending(x => x.Product!.Name),
                ("Product", _) => allSubProjects.OrderBy(x => x.Product!.Name),
                ("Owner", "desc") => allSubProjects.OrderByDescending(x => x.ModuleOwner),
                ("Owner", _) => allSubProjects.OrderBy(x => x.ModuleOwner),
                ("Status", "desc") => allSubProjects.OrderByDescending(x => x.Status).ThenBy(x => x.Name),
                ("Status", _) => allSubProjects.OrderBy(x => x.Status).ThenBy(x => x.Name),
                ("Name", "desc") => allSubProjects.OrderByDescending(x => x.Name),
                _ => allSubProjects.OrderBy(x => x.Name)
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

            SubProjects = sortedArray
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToArray();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            SubProjects = [];
            AvailableStatuses = [];
            AvailableProducts = [];
            TotalCount = 0;
            TotalPages = 0;
        }
    }
}

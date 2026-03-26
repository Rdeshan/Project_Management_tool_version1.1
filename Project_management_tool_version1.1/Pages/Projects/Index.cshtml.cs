using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Projects;

public class IndexModel(IProjectService projectService) : PageModel
{
    public IReadOnlyCollection<Project> Projects { get; private set; } = [];
    public IReadOnlyCollection<string> AvailableStatuses { get; private set; } = [];
    public IReadOnlyCollection<int> PageSizeOptions { get; } = [10, 25, 50];

    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

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
            AvailableStatuses = Enum.GetNames<ProjectStatus>().OrderBy(x => x).ToArray();

            var allProjects = await projectService.GetAllAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var normalizedSearch = Search.Trim();
                allProjects = allProjects
                    .Where(x =>
                        x.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        x.Code.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        (x.ClientName != null && x.ClientName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase)))
                    .ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Status) && Enum.TryParse<ProjectStatus>(Status, out var selectedStatus))
            {
                allProjects = allProjects.Where(x => x.Status == selectedStatus).ToArray();
            }

            if (!PageSizeOptions.Contains(PageSize))
            {
                PageSize = 10;
            }

            var sorted = (SortBy, SortDirection.ToLowerInvariant()) switch
            {
                ("Code", "desc") => allProjects.OrderByDescending(x => x.Code),
                ("Code", _) => allProjects.OrderBy(x => x.Code),
                ("Client", "desc") => allProjects.OrderByDescending(x => x.ClientName),
                ("Client", _) => allProjects.OrderBy(x => x.ClientName),
                ("Status", "desc") => allProjects.OrderByDescending(x => x.Status).ThenBy(x => x.Name),
                ("Status", _) => allProjects.OrderBy(x => x.Status).ThenBy(x => x.Name),
                ("StartDate", "desc") => allProjects.OrderByDescending(x => x.StartDate).ThenBy(x => x.Name),
                ("StartDate", _) => allProjects.OrderBy(x => x.StartDate).ThenBy(x => x.Name),
                ("Name", "desc") => allProjects.OrderByDescending(x => x.Name),
                _ => allProjects.OrderBy(x => x.Name)
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

            Projects = sortedArray
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToArray();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Projects = [];
            AvailableStatuses = [];
            TotalCount = 0;
            TotalPages = 0;
        }
    }
}

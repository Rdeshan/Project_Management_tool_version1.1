using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Products;

public class IndexModel(IProductService productService) : PageModel
{
    public IReadOnlyCollection<Product> Products { get; private set; } = [];
    public IReadOnlyCollection<string> AvailableStatuses { get; private set; } = [];
    public IReadOnlyCollection<string> AvailableProjects { get; private set; } = [];
    public IReadOnlyCollection<int> PageSizeOptions { get; } = [10, 25, 50];

    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Project { get; set; }

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
            var allProducts = await productService.GetAllAsync(cancellationToken);

            AvailableStatuses = Enum.GetNames<ProductStatus>().OrderBy(x => x).ToArray();
            AvailableProjects = allProducts
                .Select(x => x.Project?.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToArray();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var normalizedSearch = Search.Trim();
                allProducts = allProducts
                    .Where(x =>
                        x.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        x.VersionName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                        (x.Project?.Name?.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Status) && Enum.TryParse<ProductStatus>(Status, out var selectedStatus))
            {
                allProducts = allProducts.Where(x => x.Status == selectedStatus).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(Project))
            {
                allProducts = allProducts
                    .Where(x => string.Equals(x.Project?.Name, Project, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }

            if (!PageSizeOptions.Contains(PageSize))
            {
                PageSize = 10;
            }

            var sorted = (SortBy, SortDirection.ToLowerInvariant()) switch
            {
                ("Version", "desc") => allProducts.OrderByDescending(x => x.VersionName),
                ("Version", _) => allProducts.OrderBy(x => x.VersionName),
                ("Project", "desc") => allProducts.OrderByDescending(x => x.Project!.Name),
                ("Project", _) => allProducts.OrderBy(x => x.Project!.Name),
                ("Status", "desc") => allProducts.OrderByDescending(x => x.Status).ThenBy(x => x.Name),
                ("Status", _) => allProducts.OrderBy(x => x.Status).ThenBy(x => x.Name),
                ("PlannedRelease", "desc") => allProducts.OrderByDescending(x => x.PlannedReleaseDate).ThenBy(x => x.Name),
                ("PlannedRelease", _) => allProducts.OrderBy(x => x.PlannedReleaseDate).ThenBy(x => x.Name),
                ("Name", "desc") => allProducts.OrderByDescending(x => x.Name),
                _ => allProducts.OrderBy(x => x.Name)
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

            Products = sortedArray
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToArray();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Products = [];
            AvailableStatuses = [];
            AvailableProjects = [];
            TotalCount = 0;
            TotalPages = 0;
        }
    }
}

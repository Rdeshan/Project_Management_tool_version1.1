using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Products;

public class CreateModel(IProductService productService, IProjectService projectService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyCollection<SelectListItem> Projects { get; private set; } = [];
    public IReadOnlyCollection<SelectListItem> Statuses { get; } = Enum
        .GetValues<ProductStatus>()
        .Select(x => new SelectListItem(x.ToString(), x.ToString()))
        .ToArray();

    public class InputModel
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VersionName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? PlannedReleaseDate { get; set; }

        [Required]
        public ProductStatus Status { get; set; } = ProductStatus.Planned;

        [Required]
        public Guid ProjectId { get; set; }
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadProjectsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadProjectsAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        await productService.CreateAsync(
            Input.Name,
            Input.VersionName,
            Input.Description,
            Input.PlannedReleaseDate,
            Input.Status,
            Input.ProjectId,
            cancellationToken);

        return RedirectToPage("/Products/Index");
    }

    private async Task LoadProjectsAsync(CancellationToken cancellationToken)
    {
        Projects = (await projectService.GetAllAsync(cancellationToken))
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem($"{x.Code} - {x.Name}", x.Id.ToString()))
            .ToArray();
    }
}

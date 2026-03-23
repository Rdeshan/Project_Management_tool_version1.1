using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.SubProjects;

public class EditModel(ISubProjectService subProjectService, IProductService productService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyCollection<SelectListItem> Products { get; private set; } = [];
    public IReadOnlyCollection<SelectListItem> Statuses { get; } = Enum
        .GetValues<SubProjectStatus>()
        .Select(x => new SelectListItem(x.ToString(), x.ToString()))
        .ToArray();

    public class InputModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(120)]
        public string? ModuleOwner { get; set; }

        [Required]
        public SubProjectStatus Status { get; set; } = SubProjectStatus.NotStarted;

        [Required]
        public Guid ProductId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        await LoadProductsAsync(cancellationToken);

        var subProject = await subProjectService.GetByIdAsync(id, cancellationToken);
        if (subProject is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = subProject.Id,
            Name = subProject.Name,
            Description = subProject.Description,
            ModuleOwner = subProject.ModuleOwner,
            Status = subProject.Status,
            ProductId = subProject.ProductId
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadProductsAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        await subProjectService.UpdateAsync(
            Input.Id,
            Input.Name,
            Input.Description,
            Input.ModuleOwner,
            Input.Status,
            Input.ProductId,
            cancellationToken);

        return RedirectToPage("/SubProjects/Index");
    }

    private async Task LoadProductsAsync(CancellationToken cancellationToken)
    {
        Products = (await productService.GetAllAsync(cancellationToken))
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem($"{x.Name} ({x.VersionName})", x.Id.ToString()))
            .ToArray();
    }
}

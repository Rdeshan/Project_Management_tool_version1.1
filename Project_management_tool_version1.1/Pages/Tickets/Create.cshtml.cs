using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementTool.Application.Interfaces;
using ProjectManagementTool.Domain.Entities;

namespace Project_management_tool_version1._1.Pages.Tickets;

public class CreateModel(
    ITicketService ticketService,
    IProjectService projectService,
    IProductService productService,
    ISubProjectService subProjectService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyCollection<SelectListItem> Projects { get; private set; } = [];
    public IReadOnlyCollection<SelectListItem> Products { get; private set; } = [];
    public IReadOnlyCollection<SelectListItem> SubProjects { get; private set; } = [];
    public IReadOnlyCollection<SelectListItem> Categories { get; } = Enum
        .GetValues<TicketCategory>()
        .Select(x => new SelectListItem(x.ToString(), x.ToString()))
        .ToArray();
    public IReadOnlyCollection<SelectListItem> Priorities { get; } = Enum
        .GetValues<TicketPriority>()
        .Select(x => new SelectListItem(x.ToString(), x.ToString()))
        .ToArray();
    public IReadOnlyCollection<SelectListItem> Statuses { get; } = Enum
        .GetValues<TicketStatus>()
        .Select(x => new SelectListItem(x.ToString(), x.ToString()))
        .ToArray();

    public class InputModel
    {
        [Required]
        [MaxLength(40)]
        public string TicketNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(4000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TicketCategory Category { get; set; } = TicketCategory.Task;

        [Required]
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.Open;

        [Required]
        public Guid? ProjectId { get; set; }

        public Guid? ProductId { get; set; }

        public Guid? SubProjectId { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? ExpectedDueDate { get; set; }
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadLookupsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadLookupsAsync(cancellationToken);

        if (!Input.ProjectId.HasValue)
        {
            ModelState.AddModelError(nameof(Input.ProjectId), "Project is required.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        await ticketService.CreateAsync(
            Input.TicketNumber,
            Input.Title,
            Input.Description,
            Input.Category,
            Input.Priority,
            Input.Status,
            Input.ProjectId!.Value,
            Input.ProductId,
            Input.SubProjectId,
            Input.ExpectedDueDate,
            cancellationToken);

        return RedirectToPage("/Tickets/Index");
    }

    private async Task LoadLookupsAsync(CancellationToken cancellationToken)
    {
        Projects = (await projectService.GetAllAsync(cancellationToken))
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem($"{x.Code} - {x.Name}", x.Id.ToString()))
            .ToArray();

        Products = (await productService.GetAllAsync(cancellationToken))
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem($"{x.Name} ({x.VersionName})", x.Id.ToString()))
            .ToArray();

        SubProjects = (await subProjectService.GetAllAsync(cancellationToken))
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToArray();
    }
}

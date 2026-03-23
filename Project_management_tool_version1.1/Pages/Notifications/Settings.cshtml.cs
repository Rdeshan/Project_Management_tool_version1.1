using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_management_tool_version1._1.Pages.Notifications;

public class SettingsModel : PageModel
{
    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public List<EventSettingInput> Events { get; set; } = [];

    public IReadOnlyCollection<string> RecipientTypes { get; } =
    [
        "Assignee",
        "Reporter",
        "Watchers",
        "Team Lead",
        "Project Manager",
        "Director",
        "Specific Role",
        "Specific Team",
        "Specific User(s)",
        "External Email"
    ];

    public void OnGet()
    {
        EnsureDefaults();
    }

    public IActionResult OnPost()
    {
        EnsureDefaults();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        StatusMessage = "Notification settings were updated.";
        return RedirectToPage("/Notifications/Settings");
    }

    private void EnsureDefaults()
    {
        if (Events.Count > 0)
        {
            return;
        }

        Events =
        [
            new EventSettingInput { EventName = "Ticket Created", RecipientType = "Assignee", InAppEnabled = true, EmailEnabled = true, DeliveryMode = DeliveryMode.Immediate },
            new EventSettingInput { EventName = "Ticket Assigned", RecipientType = "Assignee", InAppEnabled = true, EmailEnabled = true, DeliveryMode = DeliveryMode.Immediate },
            new EventSettingInput { EventName = "Ticket Status Changed", RecipientType = "Watchers", InAppEnabled = true, EmailEnabled = true, DeliveryMode = DeliveryMode.DailyDigest },
            new EventSettingInput { EventName = "Ticket Overdue", RecipientType = "Project Manager", InAppEnabled = true, EmailEnabled = true, DeliveryMode = DeliveryMode.Immediate, EscalationDelayBusinessDays = 0, RepeatIntervalBusinessDays = 2 },
            new EventSettingInput { EventName = "Ticket Overdue — 3+ Days", RecipientType = "Director", InAppEnabled = true, EmailEnabled = true, DeliveryMode = DeliveryMode.Immediate, EscalationDelayBusinessDays = 3, RepeatIntervalBusinessDays = 2 },
            new EventSettingInput { EventName = "Sprint Overrun Risk", RecipientType = "Project Manager", InAppEnabled = true, EmailEnabled = true, DeliveryMode = DeliveryMode.Immediate, EscalationDelayBusinessDays = 0, RepeatIntervalBusinessDays = 1 }
        ];
    }

    public sealed class EventSettingInput
    {
        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public string RecipientType { get; set; } = "Assignee";

        public bool InAppEnabled { get; set; } = true;

        public bool EmailEnabled { get; set; } = true;

        public DeliveryMode DeliveryMode { get; set; } = DeliveryMode.Immediate;

        [Range(0, 30)]
        public int EscalationDelayBusinessDays { get; set; } = 0;

        [Range(1, 30)]
        public int RepeatIntervalBusinessDays { get; set; } = 2;

        [MaxLength(200)]
        public string? SubjectTemplate { get; set; } = "{ticket_id} {ticket_title} - {status}";
    }

    public enum DeliveryMode
    {
        Immediate = 1,
        DailyDigest = 2
    }
}

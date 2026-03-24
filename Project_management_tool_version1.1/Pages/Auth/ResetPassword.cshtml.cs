using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using Project_management_tool_version1._1.Security;

namespace Project_management_tool_version1._1.Pages.Auth;

public class ResetPasswordModel(IUserService userService, PasswordResetService passwordResetService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    [Required]
    public string Token { get; set; } = string.Empty;

    [BindProperty]
    public ResetPasswordInput Input { get; set; } = new();

    public bool IsCompleted { get; private set; }

    public class ResetPasswordInput
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public IActionResult OnGet()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token))
        {
            return RedirectToPage("/Auth/ForgotPassword");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token))
        {
            ModelState.AddModelError(string.Empty, "Reset link is invalid.");
            return Page();
        }

        var user = await userService.GetActiveByEmailAsync(Email, cancellationToken);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Reset link is invalid or has expired.");
            return Page();
        }

        var isValidToken = passwordResetService.ValidateToken(Email, Token);
        if (!isValidToken)
        {
            ModelState.AddModelError(string.Empty, "Reset link is invalid or has expired.");
            return Page();
        }

        await userService.UpdatePasswordAsync(user.Id, Input.Password, cancellationToken);

        IsCompleted = true;
        return Page();
    }
}

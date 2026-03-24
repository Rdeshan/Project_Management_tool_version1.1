using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagementTool.Application.Interfaces;
using Project_management_tool_version1._1.Security;
using Project_management_tool_version1._1.Services;

namespace Project_management_tool_version1._1.Pages.Auth;

public class ForgotPasswordModel(
    IUserService userService,
    PasswordResetService passwordResetService,
    IEmailSender emailSender,
    IWebHostEnvironment environment) : PageModel
{
    [BindProperty]
    public ForgotPasswordInput Input { get; set; } = new();

    public bool IsSubmitted { get; private set; }
    public string? DevResetLink { get; private set; }

    public class ForgotPasswordInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var normalizedEmail = Input.Email.Trim().ToLowerInvariant();
        var user = await userService.GetActiveByEmailAsync(normalizedEmail, cancellationToken);

        if (user is not null)
        {
            var token = passwordResetService.CreateToken(user.Email);
            var resetUrl = Url.Page("/Auth/ResetPassword", pageHandler: null, values: new { email = user.Email, token }, protocol: Request.Scheme);

            if (!string.IsNullOrWhiteSpace(resetUrl))
            {
                await emailSender.SendAsync(
                    user.Email,
                    "Password reset request",
                    $"Use this link to reset your password: <a href=\"{resetUrl}\">Reset password</a>",
                    cancellationToken);

                if (environment.IsDevelopment())
                {
                    DevResetLink = resetUrl;
                }
            }
        }

        IsSubmitted = true;
        return Page();
    }
}

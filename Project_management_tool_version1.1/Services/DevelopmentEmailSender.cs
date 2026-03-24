using Microsoft.Extensions.Logging;

namespace Project_management_tool_version1._1.Services;

public class DevelopmentEmailSender(ILogger<DevelopmentEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Development email => To: {ToEmail} | Subject: {Subject} | Body: {Body}", toEmail, subject, htmlBody);
        return Task.CompletedTask;
    }
}

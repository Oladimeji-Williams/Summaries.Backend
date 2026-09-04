namespace Summaries.Application.Abstractions.Email;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken);

    Task SendPasswordResetAsync(string toEmail, string resetLink, CancellationToken cancellationToken);
}
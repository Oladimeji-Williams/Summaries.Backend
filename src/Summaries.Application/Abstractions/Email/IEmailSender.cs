namespace Summaries.Application.Abstractions.Email;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken);

    Task SendPasswordResetAsync(
        string toEmail, string resetLink, DateTimeOffset sentAt, CancellationToken cancellationToken);

    Task SendNotificationAsync(
        string toEmail, string subject, string title, string message,
        string? actionUrl, string? actionLabel, DateTimeOffset sentAt, CancellationToken cancellationToken);
}
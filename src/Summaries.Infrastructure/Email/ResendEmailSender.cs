using Microsoft.Extensions.Options;
using Resend;
using Summaries.Application.Abstractions.Email;

namespace Summaries.Infrastructure.Email;

internal sealed class ResendEmailSender(
    IResend resendClient,
    IOptions<EmailOptions> emailOptions,
    IOptions<BrandingOptions> brandingOptions)
    : IEmailSender
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;
    private readonly BrandingOptions _brandingOptions = brandingOptions.Value;

    public async Task SendAsync(
        string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        var message = new EmailMessage
        {
            From = $"{_emailOptions.FromName} <{_emailOptions.FromAddress}>",
            Subject = subject,
            HtmlBody = htmlBody,
        };
        message.To.Add(toEmail);

        await resendClient.EmailSendAsync(message, cancellationToken);
    }

    public Task SendPasswordResetAsync(
        string toEmail, string resetLink, DateTimeOffset sentAt, CancellationToken cancellationToken)
    {
        var html = EmailTemplates.PasswordReset(resetLink, _brandingOptions.LogoUrl, sentAt);
        return SendAsync(toEmail, "Reset your Summaries password", html, cancellationToken);
    }

    public Task SendNotificationAsync(
        string toEmail, string subject, string title, string message,
        string? actionUrl, string? actionLabel, DateTimeOffset sentAt, CancellationToken cancellationToken)
    {
        var html = EmailTemplates.Notification(
            _brandingOptions.LogoUrl, title, message, actionUrl, actionLabel, sentAt);
        return SendAsync(toEmail, subject, html, cancellationToken);
    }
}
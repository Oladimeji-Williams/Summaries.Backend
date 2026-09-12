using Microsoft.Extensions.Options;
using Resend;
using Summaries.Application.Abstractions.Email;

namespace Summaries.Infrastructure.Email;

internal sealed class ResendEmailSender(
    IResend resendClient,
    IOptions<EmailOptions> emailOptions,
    IOptions<BrandingOptions> brandingOptions,
    HttpClient httpClient)
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

    public Task SendEmailConfirmationAsync(
        string toEmail, string confirmLink, DateTimeOffset sentAt, CancellationToken cancellationToken)
    {
        var html = EmailTemplates.EmailConfirmation(confirmLink, _brandingOptions.LogoUrl, sentAt);
        return SendAsync(toEmail, "Confirm your Summaries email", html, cancellationToken);
    }

    public async Task SendBookDeliveryAsync(
        string toEmail, string bookTitle, string pdfUrl, DateTimeOffset sentAt, CancellationToken cancellationToken)
    {
        var html = EmailTemplates.BookDelivery(bookTitle, pdfUrl, _brandingOptions.LogoUrl, sentAt);

        var message = new EmailMessage
        {
            From = $"{_emailOptions.FromName} <{_emailOptions.FromAddress}>",
            Subject = $"Your copy of \"{bookTitle}\"",
            HtmlBody = html,
        };
        message.To.Add(toEmail);

        try
        {
            var pdfBytes = await httpClient.GetByteArrayAsync(pdfUrl, cancellationToken);
            var fileName = SanitizeFileName(bookTitle) + ".pdf";

            message.Attachments =
            [
                new EmailAttachment
                {
                    Filename = fileName,
                    Content = pdfBytes,
                },
            ];
        }
        catch
        {
            // If the PDF can't be fetched for attachment (size limit, transient network issue),
            // still send the email with the download link in the body — never block delivery
            // of the notification itself over the attachment being best-effort.
        }

        await resendClient.EmailSendAsync(message, cancellationToken);
    }

    private static string SanitizeFileName(string title)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(title.Where(c => !invalid.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? "book" : cleaned;
    }

    public Task SendSignInCodeAsync(
        string toEmail, string code, string magicLink, DateTimeOffset sentAt, CancellationToken cancellationToken)
    {
        var html = EmailTemplates.SignInCode(code, magicLink, _brandingOptions.LogoUrl, sentAt);
        return SendAsync(toEmail, "Your Summaries sign-in code", html, cancellationToken);
    }
}
using Microsoft.Extensions.Options;
using Resend;
using Summaries.Application.Abstractions.Email;

namespace Summaries.Infrastructure.Email;

internal sealed class ResendEmailSender(IResend resendClient, IOptions<EmailOptions> options) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(
        string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        var message = new EmailMessage
        {
            From = $"{_options.FromName} <{_options.FromAddress}>",
            Subject = subject,
            HtmlBody = htmlBody,
        };
        message.To.Add(toEmail);

        await resendClient.EmailSendAsync(message, cancellationToken);
    }

    public Task SendPasswordResetAsync(
        string toEmail, string resetLink, CancellationToken cancellationToken)
    {
        var html = EmailTemplates.PasswordReset(resetLink);
        return SendAsync(toEmail, "Reset your Summaries password", html, cancellationToken);
    }
}
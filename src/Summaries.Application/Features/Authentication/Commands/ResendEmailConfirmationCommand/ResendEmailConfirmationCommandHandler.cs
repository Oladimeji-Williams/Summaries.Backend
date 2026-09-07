using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Email;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationCommandHandler(
    IIdentityService identityService,
    IEmailSender emailSender,
    TimeProvider timeProvider)
    : IRequestHandler<ResendEmailConfirmationCommand, Result>
{
    public async Task<Result> Handle(
        ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        // Always succeed regardless of whether the account exists or is
        // already confirmed — same anti-enumeration stance as ForgotPassword.
        var token = await identityService.GenerateEmailConfirmationTokenAsync(request.Email, cancellationToken);
        if (token is not null)
        {
            var confirmLink =
                $"{request.ConfirmEmailUrlBase}?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(token)}";
            var sentAt = timeProvider.GetUtcNow().ToOffset(TimeSpan.FromHours(1));
            await emailSender.SendEmailConfirmationAsync(request.Email, confirmLink, sentAt, cancellationToken);
        }

        return Result.Success();
    }
}
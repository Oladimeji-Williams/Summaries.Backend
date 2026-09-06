using MediatR;
using Microsoft.Extensions.Logging;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Email;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;
using Summaries.Application.Features.Authentication.Shared.Errors;
using Summaries.Application.Features.Authentication.Shared.Mappings;

namespace Summaries.Application.Features.Authentication.Commands.LoginCommand;

public sealed class LoginCommandHandler(
    IIdentityService identityService,
    IEmailSender emailSender,
    TimeProvider timeProvider,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(
        LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.LoginAsync(
            request.Email, request.Password, cancellationToken);

        if (result is null)
        {
            return Result<AuthResultDto>.Failure(AuthErrors.InvalidCredentials());
        }

        await TrySendLoginNotificationAsync(result.Email, result.DisplayName, timeProvider.GetUtcNow(), cancellationToken);

        return Result<AuthResultDto>.Success(result.ToDto());
    }

    private async Task TrySendLoginNotificationAsync(
        string email, string displayName, DateTimeOffset loggedInAtUtc, CancellationToken cancellationToken)
    {
        try
        {
            var sentAt = loggedInAtUtc.ToOffset(TimeSpan.FromHours(1));
            await emailSender.SendNotificationAsync(
                email,
                "New login to your Summaries account",
                "New login detected",
                $"Hi {displayName}, we noticed a new login to your Summaries account.",
                actionUrl: null,
                actionLabel: null,
                sentAt,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send login notification email to {Email}", email);
        }
    }
}
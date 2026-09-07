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
    private static readonly TimeSpan NotificationSuppressionWindow = TimeSpan.FromMinutes(15);

    public async Task<Result<AuthResultDto>> Handle(
        LoginCommand request, CancellationToken cancellationToken)
    {
        var outcome = await identityService.LoginAsync(request.Email, request.Password, cancellationToken);

        return outcome switch
        {
            LoginOutcome.Success success => await HandleSuccessAsync(success.Result, cancellationToken),
            LoginOutcome.RequiresTwoFactor twoFactor => Result<AuthResultDto>.Success(
                new AuthResultDto(
                    "", "", default, default, Guid.Empty, "", "", [], null,
                    RequiresTwoFactor: true, TwoFactorToken: twoFactor.TwoFactorToken)),
            LoginOutcome.EmailNotConfirmed => Result<AuthResultDto>.Failure(AuthErrors.EmailNotConfirmed()),
            _ => Result<AuthResultDto>.Failure(AuthErrors.InvalidCredentials()),
        };
    }

    private async Task<Result<AuthResultDto>> HandleSuccessAsync(
        AuthenticationResult result, CancellationToken cancellationToken)
    {
        await TrySendThrottledLoginNotificationAsync(result, cancellationToken);
        return Result<AuthResultDto>.Success(result.ToDto());
    }

    private async Task TrySendThrottledLoginNotificationAsync(
        AuthenticationResult result, CancellationToken cancellationToken)
    {
        try
        {
            var now = timeProvider.GetUtcNow();
            var lastSent = await identityService.GetLastLoginNotificationSentAtAsync(result.UserId, cancellationToken);

            if (lastSent is not null && now - lastSent.Value < NotificationSuppressionWindow)
            {
                return;
            }

            var sentAt = now.ToOffset(TimeSpan.FromHours(1));
            await emailSender.SendNotificationAsync(
                result.Email,
                "New login to your Summaries account",
                "New login detected",
                $"Hi {result.DisplayName}, we noticed a new login to your Summaries account.",
                actionUrl: null,
                actionLabel: null,
                sentAt,
                cancellationToken);

            await identityService.RecordLoginNotificationSentAsync(result.UserId, now, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send login notification email to {Email}", result.Email);
        }
    }
}
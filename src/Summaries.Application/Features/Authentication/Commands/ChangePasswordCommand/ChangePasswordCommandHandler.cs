using MediatR;
using Microsoft.Extensions.Logging;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Email;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Authentication.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IEmailSender emailSender,
    TimeProvider timeProvider,
    ILogger<ChangePasswordCommandHandler> logger)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure(UserErrors.NotAuthenticated());
        }

        var result = await identityService.ChangePasswordAsync(
            currentUser.UserId.Value, request.CurrentPassword, request.NewPassword, cancellationToken);

        if (result.IsSuccess)
        {
            var profile = await identityService.GetProfileAsync(currentUser.UserId.Value, cancellationToken);
            if (profile is not null)
            {
                await TrySendNotificationAsync(profile.Email, profile.FirstName, cancellationToken);
            }
        }

        return result;
    }

    private async Task TrySendNotificationAsync(string email, string firstName, CancellationToken cancellationToken)
    {
        try
        {
            var sentAt = timeProvider.GetUtcNow().ToOffset(TimeSpan.FromHours(1));
            await emailSender.SendNotificationAsync(
                email,
                "Your Summaries password was changed",
                "Password changed",
                $"Hi {firstName}, your Summaries account password was just changed.",
                actionUrl: null,
                actionLabel: null,
                sentAt,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send password-changed notification to {Email}", email);
        }
    }
}
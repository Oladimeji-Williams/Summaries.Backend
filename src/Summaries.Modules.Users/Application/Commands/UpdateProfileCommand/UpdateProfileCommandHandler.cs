using MediatR;
using Microsoft.Extensions.Logging;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Abstractions.Email;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Commands.UpdateProfileCommand;

public sealed class UpdateProfileCommandHandler(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IEmailSender emailSender,
    TimeProvider timeProvider,
    ILogger<UpdateProfileCommandHandler> logger)
    : IRequestHandler<UpdateProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure(UserErrors.NotAuthenticated());
        }

        var result = await identityService.UpdateProfileAsync(
            currentUser.UserId.Value, request.FirstName, request.LastName,
            request.PhoneNumber, request.Address, request.City, request.Country,
            cancellationToken);

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
                "Your Summaries profile was updated",
                "Profile updated",
                $"Hi {firstName}, your Summaries account details were just updated.",
                actionUrl: null,
                actionLabel: null,
                sentAt,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send profile-updated notification to {Email}", email);
        }
    }
}
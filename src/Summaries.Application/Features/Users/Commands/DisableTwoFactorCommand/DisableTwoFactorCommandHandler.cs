using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Commands.DisableTwoFactor;

public sealed class DisableTwoFactorCommandHandler(
    ICurrentUser currentUser, IIdentityService identityService)
    : IRequestHandler<DisableTwoFactorCommand, Result>
{
    public Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Task.FromResult(Result.Failure(UserErrors.NotAuthenticated()));
        }
        return identityService.DisableTwoFactorAsync(currentUser.UserId.Value, request.CurrentPassword, cancellationToken);
    }
}
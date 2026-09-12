using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Commands.DisableEmailSignIn;

public sealed class DisableEmailSignInCommandHandler(
    ICurrentUser currentUser, IIdentityService identityService)
    : IRequestHandler<DisableEmailSignInCommand, Result>
{
    public Task<Result> Handle(DisableEmailSignInCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Task.FromResult(Result.Failure(UserErrors.NotAuthenticated()));
        }
        return identityService.DisableEmailSignInAsync(currentUser.UserId.Value, cancellationToken);
    }
}
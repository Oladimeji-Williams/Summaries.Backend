using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Commands.DisableEmailSignIn;

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
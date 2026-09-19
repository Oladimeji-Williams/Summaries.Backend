using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Commands.EnableEmailSignIn;

public sealed class EnableEmailSignInCommandHandler(
    ICurrentUser currentUser, IIdentityService identityService)
    : IRequestHandler<EnableEmailSignInCommand, Result>
{
    public Task<Result> Handle(EnableEmailSignInCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Task.FromResult(Result.Failure(UserErrors.NotAuthenticated()));
        }
        return identityService.EnableEmailSignInAsync(currentUser.UserId.Value, cancellationToken);
    }
}
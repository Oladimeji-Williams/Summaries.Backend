using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Commands.EnableEmailSignIn;

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
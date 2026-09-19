using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Commands.DisableTwoFactorCommand;

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
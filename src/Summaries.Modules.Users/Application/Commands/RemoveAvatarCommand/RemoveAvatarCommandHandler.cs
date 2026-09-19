using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Commands.RemoveAvatarCommand;

public sealed class RemoveAvatarCommandHandler(
    ICurrentUser currentUser,
    IIdentityService identityService)
    : IRequestHandler<RemoveAvatarCommand, Result>
{
    public async Task<Result> Handle(RemoveAvatarCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure(UserErrors.NotAuthenticated());
        }

        return await identityService.RemoveAvatarAsync(currentUser.UserId.Value, cancellationToken);
    }
}
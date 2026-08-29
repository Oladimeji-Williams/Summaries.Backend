using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Commands.RemoveAvatar;

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
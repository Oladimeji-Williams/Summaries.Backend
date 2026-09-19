using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    ICurrentUser currentUser,
    IIdentityService identityService)
    : IRequestHandler<GetCurrentUserQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result<UserProfileDto>.Failure(UserErrors.NotAuthenticated());
        }

        var profile = await identityService.GetProfileAsync(currentUser.UserId.Value, cancellationToken);
        if (profile is null)
        {
            return Result<UserProfileDto>.Failure(UserErrors.NotFound(currentUser.UserId.Value));
        }

        return Result<UserProfileDto>.Success(profile);
    }
}
using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Queries.GetEmailSignInStatus;

public sealed class GetEmailSignInStatusQueryHandler(
    ICurrentUser currentUser, IIdentityService identityService)
    : IRequestHandler<GetEmailSignInStatusQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(GetEmailSignInStatusQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result<bool>.Failure(UserErrors.NotAuthenticated());
        }
        var enabled = await identityService.IsEmailSignInEnabledAsync(currentUser.UserId.Value, cancellationToken);
        return Result<bool>.Success(enabled);
    }
}
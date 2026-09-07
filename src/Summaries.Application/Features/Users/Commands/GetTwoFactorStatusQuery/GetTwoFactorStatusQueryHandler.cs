using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Queries.GetTwoFactorStatus;

public sealed class GetTwoFactorStatusQueryHandler(
    ICurrentUser currentUser, IIdentityService identityService)
    : IRequestHandler<GetTwoFactorStatusQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(GetTwoFactorStatusQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result<bool>.Failure(UserErrors.NotAuthenticated());
        }
        var enabled = await identityService.IsTwoFactorEnabledAsync(currentUser.UserId.Value, cancellationToken);
        return Result<bool>.Success(enabled);
    }
}
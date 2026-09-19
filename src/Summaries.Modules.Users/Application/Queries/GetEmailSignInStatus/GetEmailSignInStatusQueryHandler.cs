using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Queries.GetEmailSignInStatus;

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
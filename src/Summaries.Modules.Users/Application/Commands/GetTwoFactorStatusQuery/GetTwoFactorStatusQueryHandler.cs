using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Commands.GetTwoFactorStatusQuery;

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
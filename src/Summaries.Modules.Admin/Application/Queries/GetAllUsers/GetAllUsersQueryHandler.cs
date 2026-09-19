using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Admin.Application.Queries.GetAllUsers;

public sealed class GetAllUsersQueryHandler(IIdentityService identityService)
    : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<UserProfileDto>>>
{
    public async Task<Result<IReadOnlyList<UserProfileDto>>> Handle(
        GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await identityService.GetAllUsersAsync(cancellationToken);
        return Result<IReadOnlyList<UserProfileDto>>.Success(users);
    }
}
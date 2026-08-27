using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.DTOs;

namespace Summaries.Application.Features.Admin.Queries.GetAllUsers;

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
using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<Result<UserProfileDto>>;
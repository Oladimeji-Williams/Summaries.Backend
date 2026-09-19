using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Admin.Application.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IRequest<Result<IReadOnlyList<UserProfileDto>>>;
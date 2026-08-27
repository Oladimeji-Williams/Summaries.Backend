using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.DTOs;

namespace Summaries.Application.Features.Admin.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IRequest<Result<IReadOnlyList<UserProfileDto>>>;
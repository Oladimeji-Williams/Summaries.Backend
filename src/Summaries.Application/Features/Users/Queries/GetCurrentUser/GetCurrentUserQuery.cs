using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.DTOs;

namespace Summaries.Application.Features.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<Result<UserProfileDto>>;
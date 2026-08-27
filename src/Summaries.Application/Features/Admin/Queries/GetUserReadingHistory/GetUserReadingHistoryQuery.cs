using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Admin.Shared.DTOs;

namespace Summaries.Application.Features.Admin.Queries.GetUserReadingHistory;

public sealed record GetUserReadingHistoryQuery(Guid UserId)
    : IRequest<Result<UserReadingHistoryDto>>;
using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Admin.Application.DTOs;

namespace Summaries.Modules.Admin.Application.Queries.GetUserReadingHistory;

public sealed record GetUserReadingHistoryQuery(Guid UserId)
    : IRequest<Result<UserReadingHistoryDto>>;
using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Queries.GetTwoFactorStatus;

public sealed record GetTwoFactorStatusQuery : IRequest<Result<bool>>;
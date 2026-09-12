using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Queries.GetEmailSignInStatus;

public sealed record GetEmailSignInStatusQuery : IRequest<Result<bool>>;
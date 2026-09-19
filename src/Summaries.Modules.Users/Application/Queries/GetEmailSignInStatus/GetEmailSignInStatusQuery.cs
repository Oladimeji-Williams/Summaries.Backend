using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Queries.GetEmailSignInStatus;

public sealed record GetEmailSignInStatusQuery : IRequest<Result<bool>>;
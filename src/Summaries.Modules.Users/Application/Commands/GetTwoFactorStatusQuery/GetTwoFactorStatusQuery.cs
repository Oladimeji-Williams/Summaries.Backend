using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Commands.GetTwoFactorStatusQuery;

public sealed record GetTwoFactorStatusQuery : IRequest<Result<bool>>;
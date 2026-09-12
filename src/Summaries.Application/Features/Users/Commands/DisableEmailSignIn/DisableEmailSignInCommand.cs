using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Commands.DisableEmailSignIn;

public sealed record DisableEmailSignInCommand : IRequest<Result>;
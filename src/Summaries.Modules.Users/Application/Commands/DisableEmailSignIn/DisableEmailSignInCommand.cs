using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Commands.DisableEmailSignIn;

public sealed record DisableEmailSignInCommand : IRequest<Result>;
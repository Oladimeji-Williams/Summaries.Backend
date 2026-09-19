using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Commands.EnableEmailSignIn;

public sealed record EnableEmailSignInCommand : IRequest<Result>;
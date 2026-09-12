using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Commands.EnableEmailSignIn;

public sealed record EnableEmailSignInCommand : IRequest<Result>;
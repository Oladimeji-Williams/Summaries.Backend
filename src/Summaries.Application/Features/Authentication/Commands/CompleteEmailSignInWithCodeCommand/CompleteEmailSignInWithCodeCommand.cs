using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;

namespace Summaries.Application.Features.Authentication.Commands.CompleteEmailSignInWithCodeCommand;

public sealed record CompleteEmailSignInWithCodeCommand(string Email, string Code) : IRequest<Result<AuthResultDto>>;
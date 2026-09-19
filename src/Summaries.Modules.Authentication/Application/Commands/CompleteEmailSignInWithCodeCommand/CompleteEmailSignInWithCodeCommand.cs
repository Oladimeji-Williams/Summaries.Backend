using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.DTOs;

namespace Summaries.Modules.Authentication.Application.Commands.CompleteEmailSignInWithCodeCommand;

public sealed record CompleteEmailSignInWithCodeCommand(string Email, string Code) : IRequest<Result<AuthResultDto>>;
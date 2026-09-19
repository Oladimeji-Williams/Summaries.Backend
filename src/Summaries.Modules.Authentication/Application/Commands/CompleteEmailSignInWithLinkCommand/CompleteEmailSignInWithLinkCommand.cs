using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.DTOs;

namespace Summaries.Modules.Authentication.Application.Commands.CompleteEmailSignInWithLinkCommand;

public sealed record CompleteEmailSignInWithLinkCommand(string Token) : IRequest<Result<AuthResultDto>>;
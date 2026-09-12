using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;

namespace Summaries.Application.Features.Authentication.Commands.CompleteEmailSignInWithLinkCommand;

public sealed record CompleteEmailSignInWithLinkCommand(string Token) : IRequest<Result<AuthResultDto>>;
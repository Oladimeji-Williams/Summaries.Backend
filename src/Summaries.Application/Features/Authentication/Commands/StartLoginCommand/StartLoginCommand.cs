using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.StartLoginCommand;

public sealed record StartLoginCommand(string Email) : IRequest<Result<LoginStartResultDto>>;

public sealed record LoginStartResultDto(string Outcome);
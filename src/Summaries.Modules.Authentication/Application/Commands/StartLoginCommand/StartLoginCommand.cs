using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.StartLoginCommand;

public sealed record StartLoginCommand(string Email) : IRequest<Result<LoginStartResultDto>>;

public sealed record LoginStartResultDto(string Outcome);
using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.RegisterCommand;

public sealed record RegisterCommand(
    string Email, string Password, string ConfirmEmailUrlBase) : IRequest<Result<Guid>>;
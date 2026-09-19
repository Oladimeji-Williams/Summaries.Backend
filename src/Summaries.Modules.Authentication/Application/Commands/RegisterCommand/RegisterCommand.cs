using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.RegisterCommand;

public sealed record RegisterCommand(
    string Email, string Password, string ConfirmEmailUrlBase) : IRequest<Result<Guid>>;
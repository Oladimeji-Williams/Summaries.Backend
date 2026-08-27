using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.RegisterCommand;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password)
    : IRequest<Result<Guid>>;
using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.ConfirmEmailCommand;

public sealed record ConfirmEmailCommand(string Email, string Token) : IRequest<Result>;
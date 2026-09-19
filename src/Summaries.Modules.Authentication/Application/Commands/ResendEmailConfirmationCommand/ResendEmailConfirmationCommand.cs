using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.ResendEmailConfirmationCommand;

public sealed record ResendEmailConfirmationCommand(
    string Email, string ConfirmEmailUrlBase) : IRequest<Result>;
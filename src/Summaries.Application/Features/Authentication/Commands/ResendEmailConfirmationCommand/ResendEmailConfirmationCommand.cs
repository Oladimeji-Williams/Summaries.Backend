using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ResendEmailConfirmation;

public sealed record ResendEmailConfirmationCommand(
    string Email, string ConfirmEmailUrlBase) : IRequest<Result>;
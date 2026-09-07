using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Token) : IRequest<Result>;
using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email, string ResetUrlBase) : IRequest<Result>;
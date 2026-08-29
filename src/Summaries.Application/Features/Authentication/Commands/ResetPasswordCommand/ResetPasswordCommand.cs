using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email, string Token, string NewPassword) : IRequest<Result>;
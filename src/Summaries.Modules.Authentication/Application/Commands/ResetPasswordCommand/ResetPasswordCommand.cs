using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.ResetPasswordCommand;

public sealed record ResetPasswordCommand(
    string Email, string Token, string NewPassword) : IRequest<Result>;
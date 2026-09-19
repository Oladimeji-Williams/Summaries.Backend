using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.ForgotPasswordCommand;

public sealed record ForgotPasswordCommand(
    string Email,
    string ResetUrlBase) : IRequest<Result>;
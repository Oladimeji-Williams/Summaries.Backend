using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.ChangePasswordCommand;

public sealed record ChangePasswordCommand(
    string CurrentPassword, string NewPassword) : IRequest<Result>;
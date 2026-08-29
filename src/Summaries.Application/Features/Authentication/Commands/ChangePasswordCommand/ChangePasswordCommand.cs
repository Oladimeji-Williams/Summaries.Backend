using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword, string NewPassword) : IRequest<Result>;
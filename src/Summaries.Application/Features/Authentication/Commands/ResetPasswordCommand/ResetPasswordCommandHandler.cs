using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(IIdentityService identityService)
    : IRequestHandler<ResetPasswordCommand, Result>
{
    public Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return identityService.ResetPasswordAsync(
            request.Email, request.Token, request.NewPassword, cancellationToken);
    }
}
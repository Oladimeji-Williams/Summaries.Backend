using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(IIdentityService identityService)
    : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResult>>
{
    public async Task<Result<ForgotPasswordResult>> Handle(
        ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // Always succeeds, even for an unknown email — never reveal whether
        // an account exists for a given address (standard anti-enumeration practice).
        var token = await identityService.GeneratePasswordResetTokenAsync(
            request.Email, cancellationToken);

        return Result<ForgotPasswordResult>.Success(new ForgotPasswordResult(token));
    }
}
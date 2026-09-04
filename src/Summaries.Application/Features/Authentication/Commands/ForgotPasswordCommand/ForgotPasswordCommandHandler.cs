using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Email;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IIdentityService identityService,
    IEmailSender emailSender)
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Always return success so we don't reveal whether
        // the supplied email belongs to an account.
        var token = await identityService.GeneratePasswordResetTokenAsync(
            request.Email,
            cancellationToken);

        if (token is not null)
        {
            var resetLink =
                $"{request.ResetUrlBase}" +
                $"?email={Uri.EscapeDataString(request.Email)}" +
                $"&token={Uri.EscapeDataString(token)}";

            await emailSender.SendPasswordResetAsync(
                request.Email,
                resetLink,
                cancellationToken);
        }

        return Result.Success();
    }
}
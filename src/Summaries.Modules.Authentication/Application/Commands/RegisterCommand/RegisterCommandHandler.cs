using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Abstractions.Email;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.RegisterCommand;

public sealed class RegisterCommandHandler(
    IIdentityService identityService,
    IEmailSender emailSender,
    TimeProvider timeProvider)
    : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.RegisterAsync(
            string.Empty, string.Empty, request.Email, request.Password, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        var token = await identityService.GenerateEmailConfirmationTokenAsync(request.Email, cancellationToken);
        if (token is not null)
        {
            var confirmLink =
                $"{request.ConfirmEmailUrlBase}?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(token)}";
            var sentAt = timeProvider.GetUtcNow().ToOffset(TimeSpan.FromHours(1));
            await emailSender.SendEmailConfirmationAsync(request.Email, confirmLink, sentAt, cancellationToken);
        }

        return result;
    }
}
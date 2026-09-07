using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(IIdentityService identityService)
    : IRequestHandler<ConfirmEmailCommand, Result>
{
    public Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return identityService.ConfirmEmailAsync(request.Email, request.Token, cancellationToken);
    }
}
using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Commands.EnableTwoFactor;

public sealed class ConfirmTwoFactorSetupCommandHandler(
    ICurrentUser currentUser, IIdentityService identityService)
    : IRequestHandler<ConfirmTwoFactorSetupCommand, Result>
{
    public Task<Result> Handle(ConfirmTwoFactorSetupCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Task.FromResult(Result.Failure(UserErrors.NotAuthenticated()));
        }
        return identityService.ConfirmTwoFactorSetupAsync(currentUser.UserId.Value, request.Code, cancellationToken);
    }
}
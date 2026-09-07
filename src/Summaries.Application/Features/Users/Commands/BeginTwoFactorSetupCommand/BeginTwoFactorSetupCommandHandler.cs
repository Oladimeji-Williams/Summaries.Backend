using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Commands.EnableTwoFactor;

public sealed class BeginTwoFactorSetupCommandHandler(
    ICurrentUser currentUser, IIdentityService identityService)
    : IRequestHandler<BeginTwoFactorSetupCommand, Result<TwoFactorSetupResult>>
{
    public Task<Result<TwoFactorSetupResult>> Handle(
        BeginTwoFactorSetupCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Task.FromResult(Result<TwoFactorSetupResult>.Failure(UserErrors.NotAuthenticated()));
        }
        return identityService.BeginTwoFactorSetupAsync(currentUser.UserId.Value, cancellationToken);
    }
}
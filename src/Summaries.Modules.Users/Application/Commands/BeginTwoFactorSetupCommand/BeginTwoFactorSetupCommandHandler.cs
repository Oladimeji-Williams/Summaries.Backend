using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Commands.BeginTwoFactorSetupCommand;

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
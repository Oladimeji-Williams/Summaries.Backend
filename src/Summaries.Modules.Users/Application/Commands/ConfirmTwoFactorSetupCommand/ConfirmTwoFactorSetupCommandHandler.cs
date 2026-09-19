using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Users.Application.Commands.ConfirmTwoFactorSetupCommand;

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
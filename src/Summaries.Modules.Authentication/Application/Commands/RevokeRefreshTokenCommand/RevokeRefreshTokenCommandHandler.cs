using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.Errors;

namespace Summaries.Modules.Authentication.Application.Commands.RevokeRefreshTokenCommand;

public sealed class RevokeRefreshTokenCommandHandler(
    IIdentityService identityService)
    : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result> Handle(
        RevokeRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var revoked =
            await _identityService.RevokeRefreshTokenAsync(
                request.RefreshToken,
                cancellationToken);

        if (!revoked)
        {
            return Result.Failure(
                AuthErrors.RefreshTokenInvalid());
        }

        return Result.Success();
    }
}
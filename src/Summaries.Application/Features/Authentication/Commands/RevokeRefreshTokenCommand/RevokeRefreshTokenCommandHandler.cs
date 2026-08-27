using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.Errors;

namespace Summaries.Application.Features.Authentication.Commands.RevokeRefreshTokenCommand;

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
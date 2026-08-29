using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;
using Summaries.Application.Features.Authentication.Shared.Errors;

namespace Summaries.Application.Features.Authentication.Commands.RefreshTokenCommand;

public sealed class RefreshTokenCommandHandler(IIdentityService identityService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(
        RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.RefreshTokenAsync(
            request.RefreshToken, cancellationToken);

        if (result is null)
        {
            return Result<AuthResultDto>.Failure(AuthErrors.RefreshTokenInvalid());
        }

        return Result<AuthResultDto>.Success(
            new AuthResultDto(
                result.AccessToken, result.RefreshToken,
                result.AccessTokenExpiresAtUtc, result.RefreshTokenExpiresAtUtc,
                result.UserId, result.Email, result.DisplayName, result.Roles, result.AvatarUrl));
    }
}
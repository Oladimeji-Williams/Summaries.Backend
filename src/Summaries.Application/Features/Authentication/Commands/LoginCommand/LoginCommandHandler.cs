using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;
using Summaries.Application.Features.Authentication.Shared.Errors;

namespace Summaries.Application.Features.Authentication.Commands.LoginCommand;

public sealed class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(
        LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.LoginAsync(
            request.Email, request.Password, cancellationToken);

        if (result is null)
        {
            return Result<AuthResultDto>.Failure(AuthErrors.InvalidCredentials());
        }

        return Result<AuthResultDto>.Success(
            new AuthResultDto(
                result.AccessToken, result.RefreshToken,
                result.AccessTokenExpiresAtUtc, result.RefreshTokenExpiresAtUtc,
                result.UserId, result.Email, result.DisplayName, result.Roles));
    }
}
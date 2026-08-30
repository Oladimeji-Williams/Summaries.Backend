using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;
using Summaries.Application.Features.Authentication.Shared.Errors;
using Summaries.Application.Features.Authentication.Shared.Mappings;

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

        return Result<AuthResultDto>.Success(result.ToDto());
    }
}
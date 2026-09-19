using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.DTOs;
using Summaries.Modules.Authentication.Application.Errors;
using Summaries.Modules.Authentication.Application.Mappings;

namespace Summaries.Modules.Authentication.Application.Commands.RefreshTokenCommand;

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
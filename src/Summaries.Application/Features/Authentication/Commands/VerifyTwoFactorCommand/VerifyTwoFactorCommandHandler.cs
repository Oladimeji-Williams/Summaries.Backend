using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;
using Summaries.Application.Features.Authentication.Shared.Errors;
using Summaries.Application.Features.Authentication.Shared.Mappings;

namespace Summaries.Application.Features.Authentication.Commands.VerifyTwoFactor;

public sealed class VerifyTwoFactorCommandHandler(IIdentityService identityService)
    : IRequestHandler<VerifyTwoFactorCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(
        VerifyTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var outcome = await identityService.VerifyTwoFactorCodeAsync(
            request.TwoFactorToken, request.Code, cancellationToken);

        return outcome switch
        {
            LoginOutcome.Success success => Result<AuthResultDto>.Success(success.Result.ToDto()),
            _ => Result<AuthResultDto>.Failure(AuthErrors.InvalidTwoFactorCode()),
        };
    }
}
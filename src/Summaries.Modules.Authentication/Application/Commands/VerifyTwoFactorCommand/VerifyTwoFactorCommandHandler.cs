using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.DTOs;
using Summaries.Modules.Authentication.Application.Errors;
using Summaries.Modules.Authentication.Application.Mappings;

namespace Summaries.Modules.Authentication.Application.Commands.VerifyTwoFactorCommand;

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
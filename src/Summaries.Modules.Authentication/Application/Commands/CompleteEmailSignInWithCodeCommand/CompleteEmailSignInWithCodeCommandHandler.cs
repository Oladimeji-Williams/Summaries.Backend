using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.DTOs;
using Summaries.Modules.Authentication.Application.Errors;
using Summaries.Modules.Authentication.Application.Mappings;

namespace Summaries.Modules.Authentication.Application.Commands.CompleteEmailSignInWithCodeCommand;

public sealed class CompleteEmailSignInWithCodeCommandHandler(IIdentityService identityService)
    : IRequestHandler<CompleteEmailSignInWithCodeCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(
        CompleteEmailSignInWithCodeCommand request, CancellationToken cancellationToken)
    {
        var outcome = await identityService.CompleteEmailSignInWithCodeAsync(request.Email, request.Code, cancellationToken);
        return outcome switch
        {
            LoginOutcome.Success success => Result<AuthResultDto>.Success(success.Result.ToDto()),
            _ => Result<AuthResultDto>.Failure(AuthErrors.InvalidSignInCode()),
        };
    }
}
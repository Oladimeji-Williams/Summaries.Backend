using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;
using Summaries.Application.Features.Authentication.Shared.Errors;
using Summaries.Application.Features.Authentication.Shared.Mappings;

namespace Summaries.Application.Features.Authentication.Commands.CompleteEmailSignInWithCodeCommand;

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
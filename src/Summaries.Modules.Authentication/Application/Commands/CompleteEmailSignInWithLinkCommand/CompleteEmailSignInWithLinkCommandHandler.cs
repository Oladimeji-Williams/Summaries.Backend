using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.DTOs;
using Summaries.Modules.Authentication.Application.Errors;
using Summaries.Modules.Authentication.Application.Mappings;

namespace Summaries.Modules.Authentication.Application.Commands.CompleteEmailSignInWithLinkCommand;

public sealed class CompleteEmailSignInWithLinkCommandHandler(IIdentityService identityService)
    : IRequestHandler<CompleteEmailSignInWithLinkCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(
        CompleteEmailSignInWithLinkCommand request, CancellationToken cancellationToken)
    {
        var outcome = await identityService.CompleteEmailSignInWithLinkAsync(request.Token, cancellationToken);
        return outcome switch
        {
            LoginOutcome.Success success => Result<AuthResultDto>.Success(success.Result.ToDto()),
            _ => Result<AuthResultDto>.Failure(AuthErrors.InvalidSignInCode()),
        };
    }
}
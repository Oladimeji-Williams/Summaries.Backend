using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;
using Summaries.Application.Features.Authentication.Shared.Errors;
using Summaries.Application.Features.Authentication.Shared.Mappings;

namespace Summaries.Application.Features.Authentication.Commands.CompleteEmailSignInWithLinkCommand;

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
using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.StartLoginCommand;

public sealed class StartLoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<StartLoginCommand, Result<LoginStartResultDto>>
{
    public async Task<Result<LoginStartResultDto>> Handle(StartLoginCommand request, CancellationToken cancellationToken)
    {
        var outcome = await identityService.StartLoginAsync(request.Email, cancellationToken);
        var dto = outcome switch
        {
            LoginStartOutcome.AccountNotFound => new LoginStartResultDto("AccountNotFound"),
            LoginStartOutcome.UsePassword => new LoginStartResultDto("UsePassword"),
            LoginStartOutcome.EmailCodeSent => new LoginStartResultDto("EmailCodeSent"),
            LoginStartOutcome.NoPasswordSet => new LoginStartResultDto("NoPasswordSet"),
            LoginStartOutcome.EmailDeliveryFailed => new LoginStartResultDto("EmailDeliveryFailed"),
            _ => new LoginStartResultDto("UsePassword"),
        };
        return Result<LoginStartResultDto>.Success(dto);
    }
}
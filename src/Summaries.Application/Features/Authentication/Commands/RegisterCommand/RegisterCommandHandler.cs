using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.RegisterCommand;

public sealed class RegisterCommandHandler(
    IIdentityService identityService)
    : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        return identityService.RegisterAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            cancellationToken);
    }
}
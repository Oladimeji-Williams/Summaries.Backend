using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Commands.UpdateProfile;

public sealed class UpdateProfileCommandHandler(
    ICurrentUser currentUser,
    IIdentityService identityService)
    : IRequestHandler<UpdateProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure(UserErrors.NotAuthenticated());
        }

        return await identityService.UpdateProfileAsync(
            currentUser.UserId.Value, request.FirstName, request.LastName,
            request.PhoneNumber, request.Address, request.City, request.Country,
            cancellationToken);
    }
}
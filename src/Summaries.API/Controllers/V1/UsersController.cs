using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Summaries.API.Controllers.V1.Base;
using Summaries.Application.Features.Users.Queries.GetCurrentUser;
using Summaries.API.Contracts.Common;
using Summaries.Application.Features.Users.Shared.DTOs;

namespace Summaries.API.Controllers.V1;

[ApiVersion(1.0)]
[Authorize]
public sealed class UsersController(ISender sender) : V1ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCurrentUserQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }
}
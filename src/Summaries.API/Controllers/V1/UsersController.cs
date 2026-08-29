using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Summaries.API.Common.Urls;
using Summaries.API.Contracts.Common;
using Summaries.API.Controllers.V1.Base;
using Summaries.Application.Features.Users.Commands.UpdateProfile;
using Summaries.Application.Features.Users.Commands.UploadAvatar;
using Summaries.Application.Features.Users.Queries.GetCurrentUser;
using Summaries.Application.Features.Users.Shared.DTOs;
using Summaries.Application.Features.Users.Commands.RemoveAvatar;

namespace Summaries.API.Controllers.V1;

[ApiVersion(1.0)]
[Authorize]
public sealed class UsersController(ISender sender, IUrlBuilder urlBuilder) : V1ControllerBase
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
        var profile = result.Value! with { AvatarUrl = urlBuilder.ToAbsoluteUrl(result.Value!.AvatarUrl) };
        return Success(profile);
    }

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("me/avatar")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<IActionResult> UploadAvatar(
        IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var command = new UploadAvatarCommand(stream, file.FileName, file.ContentType, file.Length);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(urlBuilder.ToAbsoluteUrl(result.Value));
    }

    [HttpDelete("me/avatar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveAvatar(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveAvatarCommand(), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }
}
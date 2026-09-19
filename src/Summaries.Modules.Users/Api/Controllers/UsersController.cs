using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Summaries.Shared.Infrastructure.Contracts.Common;
using Summaries.Shared.Infrastructure.Api;
using Summaries.Modules.Users.Application.Commands.RemoveAvatarCommand;
using Summaries.Modules.Users.Application.Commands.UpdateProfileCommand;
using Summaries.Modules.Users.Application.Commands.UploadAvatarCommand;
using Summaries.Modules.Users.Application.Queries.GetCurrentUser;
using Summaries.SharedKernel.Contracts.Users;
using Summaries.Modules.Users.Application.Commands.BeginTwoFactorSetupCommand;
using Summaries.Modules.Users.Application.Commands.DisableTwoFactorCommand;
using Summaries.Modules.Users.Application.Commands.GetTwoFactorStatusQuery;
using Summaries.Modules.Users.Api.Contracts;
using Summaries.Modules.Users.Application.Queries.GetEmailSignInStatus;
using Summaries.Modules.Users.Application.Commands.EnableEmailSignIn;
using Summaries.Modules.Users.Application.Commands.DisableEmailSignIn;
using Summaries.Modules.Users.Application.Commands.ConfirmTwoFactorSetupCommand;

namespace Summaries.Modules.Users.Api.Controllers;

[Authorize]
public sealed class UsersController(ISender sender) : V1ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType(
        typeof(ApiResponse<UserProfileDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCurrentUserQuery(),
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return Success(result.Value);
    }

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return NoContent();
    }

    [HttpPost("me/avatar")]
    [ProducesResponseType(
        typeof(ApiResponse<string>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<IActionResult> UploadAvatar(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var command = new UploadAvatarCommand(
            stream,
            file.FileName,
            file.ContentType,
            file.Length);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return Success(result.Value);
    }

    [HttpDelete("me/avatar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveAvatar(
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RemoveAvatarCommand(),
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return NoContent();
    }

    [HttpGet("me/two-factor")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTwoFactorStatus(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTwoFactorStatusQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }

    [HttpPost("me/two-factor/setup")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BeginTwoFactorSetup(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new BeginTwoFactorSetupCommand(), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }

    [HttpPost("me/two-factor/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmTwoFactorSetup(
        [FromBody] ConfirmTwoFactorRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ConfirmTwoFactorSetupCommand(request.Code), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("me/two-factor/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DisableTwoFactor(
        [FromBody] DisableTwoFactorRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DisableTwoFactorCommand(request.CurrentPassword), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpGet("me/email-sign-in")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmailSignInStatus(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEmailSignInStatusQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }

    [HttpPost("me/email-sign-in/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EnableEmailSignIn(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new EnableEmailSignInCommand(), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("me/email-sign-in/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DisableEmailSignIn(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DisableEmailSignInCommand(), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }
}
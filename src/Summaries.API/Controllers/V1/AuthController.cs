using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Summaries.API.Contracts.Auth;
using Summaries.API.Contracts.Common;
using Summaries.API.Common.Urls;
using Summaries.API.Controllers.V1.Base;
using Summaries.Application.Features.Authentication.Commands.LoginCommand;
using Summaries.Application.Features.Authentication.Commands.RefreshTokenCommand;
using Summaries.Application.Features.Authentication.Commands.RegisterCommand;
using Summaries.Application.Features.Authentication.Commands.RevokeRefreshTokenCommand;
using Summaries.Application.Features.Authentication.Shared.DTOs;
using Summaries.Application.Features.Authentication.Commands.ForgotPassword;
using Summaries.Application.Features.Authentication.Commands.ResetPassword;
using Summaries.Application.Features.Authentication.Commands.ChangePassword;

namespace Summaries.API.Controllers.V1;

[ApiVersion(1.0)]
public sealed class AuthController(ISender sender, IUrlBuilder urlBuilder) : V1ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.FirstName, request.LastName, request.Email, request.Password);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        var response = result.Value! with
        {
            AvatarUrl = urlBuilder.ToAbsoluteUrl(result.Value!.AvatarUrl)
        };
        return Success(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        var response = result.Value! with
        {
            AvatarUrl = urlBuilder.ToAbsoluteUrl(result.Value!.AvatarUrl)
        };
        return Success(response);
    }

    [HttpPost("revoke")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Revoke(
        [FromBody] RevokeRefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RevokeRefreshTokenCommand(request.RefreshToken);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        // DEV ONLY: returning the token directly so the flow is testable without
        // an email sender. Replace with a real email send + generic ack response
        // before any real deployment — never return this token over the wire in prod.
        return Success(result.Value);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(request.CurrentPassword, request.NewPassword);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }
}

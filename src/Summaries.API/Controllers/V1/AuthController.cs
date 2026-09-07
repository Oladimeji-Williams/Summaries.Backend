using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Summaries.API.Contracts.Auth;
using Summaries.API.Contracts.Common;
using Summaries.API.Controllers.V1.Base;
using Summaries.Application.Features.Authentication.Commands.LoginCommand;
using Summaries.Application.Features.Authentication.Commands.RefreshTokenCommand;
using Summaries.Application.Features.Authentication.Commands.RegisterCommand;
using Summaries.Application.Features.Authentication.Commands.RevokeRefreshTokenCommand;
using Summaries.Application.Features.Authentication.Shared.DTOs;
using Summaries.Application.Features.Authentication.Commands.ForgotPassword;
using Summaries.Application.Features.Authentication.Commands.ResetPassword;
using Summaries.Application.Features.Authentication.Commands.ChangePassword;
using Summaries.Application.Features.Authentication.Commands.ConfirmEmail;
using Summaries.Application.Features.Authentication.Commands.ResendEmailConfirmation;
using Summaries.API.Common.RateLimiting;
using Summaries.Application.Features.Authentication.Commands.VerifyTwoFactor;

namespace Summaries.API.Controllers.V1;

public sealed class AuthController(ISender sender) : V1ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.ConfirmEmailUrlBase);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }

    [HttpPost("revoke")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Revoke(
        [FromBody] RevokeRefreshTokenRequest request, CancellationToken cancellationToken)
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
    [EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ForgotPasswordCommand(request.Email, request.ResetUrlBase), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
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

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(request.Email, request.Token);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResendConfirmation(
        [FromBody] ResendConfirmationRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ResendEmailConfirmationCommand(request.Email, request.ConfirmEmailUrlBase), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("verify-two-factor")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyTwoFactor(
        [FromBody] VerifyTwoFactorRequest request, CancellationToken cancellationToken)
    {
        var command = new VerifyTwoFactorCommand(request.TwoFactorToken, request.Code);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }
}
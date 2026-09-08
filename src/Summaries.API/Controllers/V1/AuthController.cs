using MediatR;
using Asp.Versioning;
using System.Security.Claims;
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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Summaries.API.Common;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Features.Authentication.Shared.Mappings;

namespace Summaries.API.Controllers.V1;

public sealed class AuthController(ISender sender, IIdentityService identityService,
    IMemoryCache exchangeCache,
    IOptions<FrontendOptions> frontendOptions) : V1ControllerBase
{
    private readonly FrontendOptions _frontend = frontendOptions.Value;

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

    [HttpPost("external-login/exchange")]
    [AllowAnonymous]
    public IActionResult ExchangeExternalLogin([FromBody] ExchangeExternalLoginRequest request)
    {
        if (!exchangeCache.TryGetValue(request.Code, out AuthResultDto? dto) || dto is null)
        {
            return Unauthorized(new ApiResponse<object>(false, null,
                [new ApiError("Auth.ExternalLoginExpired", "Session expired. Please try signing in again.", "Unauthorized")]));
        }
        exchangeCache.Remove(request.Code);
        return Success(dto);
    }

    [HttpGet("external-login/{provider}")]
    [AllowAnonymous]
    public IActionResult ExternalLogin(string provider)
    {
        var scheme = NormalizeScheme(provider);
        if (scheme is null)
        {
            return NotFound();
        }

        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { provider }, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, scheme);
    }
    
    [HttpGet("external-login-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string provider, CancellationToken cancellationToken)
    {
        var authResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        if (!authResult.Succeeded || authResult.Principal is null)
        {
            return Redirect($"{_frontend.BaseUrl}/login?externalError=1");
        }

        var principal = authResult.Principal;
        var providerKey = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var name = principal.FindFirstValue(ClaimTypes.Name) ?? email ?? "New User";

        if (providerKey is null || email is null)
        {
            return Redirect($"{_frontend.BaseUrl}/login?externalError=1");
        }

        var scheme = NormalizeScheme(provider) ?? provider;
        var outcome = await identityService.LoginWithExternalProviderAsync(
            scheme, providerKey, email, name, cancellationToken);

        if (outcome is not LoginOutcome.Success success)
        {
            return Redirect($"{_frontend.BaseUrl}/login?externalError=1");
        }

        var code = Guid.NewGuid().ToString("N");
        exchangeCache.Set(code, success.Result.ToDto(), TimeSpan.FromSeconds(30));

        return Redirect($"{_frontend.BaseUrl}/auth/callback?code={code}");
    }

    private static string? NormalizeScheme(string provider) => provider.ToLowerInvariant() switch
    {
        "google" => "Google",
        "microsoft" => "Microsoft",
        "facebook" => "Facebook",
        "linkedin" => "LinkedIn",
        "twitter" => "Twitter",
        _ => null,
    };

}
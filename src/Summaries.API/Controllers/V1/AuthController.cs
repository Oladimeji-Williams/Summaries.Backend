using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Summaries.API.Contracts.Auth;
using Summaries.API.Contracts.Common;
using Summaries.API.Controllers.V1.Base;
using Summaries.Application.Features.Authentication.Commands.LoginCommand;
using Summaries.Application.Features.Authentication.Commands.RefreshTokenCommand;
using Summaries.Application.Features.Authentication.Commands.RegisterCommand;
using Summaries.Application.Features.Authentication.Commands.RevokeRefreshTokenCommand;
using Summaries.Application.Features.Authentication.Shared.DTOs;

namespace Summaries.API.Controllers.V1;

[ApiVersion(1.0)]
[AllowAnonymous]
public sealed class AuthController(ISender sender) : V1ControllerBase
{
    [HttpPost("register")]
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
        return Success(result.Value);
    }

    [HttpPost("refresh")]
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
        return Success(result.Value);
    }

    [HttpPost("revoke")]
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
}
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Summaries.Application.Abstractions.Authentication;

namespace Summaries.Infrastructure.Authentication;

internal sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?
            .User
            .Identity?
            .IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var value =
                _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(
                        JwtRegisteredClaimNames.Sub)
                ??
                _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(
                        ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }
}
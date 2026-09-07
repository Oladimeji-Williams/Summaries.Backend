using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Summaries.Application.Abstractions.Authentication;

namespace Summaries.Infrastructure.Authentication;

internal sealed class JwtTokenService : ITokenService
{
    private const string TwoFactorTokenType = "2fa_pending";
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateAccessToken(
        Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer, audience: _options.Audience, claims: claims,
            expires: expires, signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Task.FromResult(Convert.ToBase64String(bytes));
    }

    public string GenerateTwoFactorToken(Guid userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("type", TwoFactorTokenType),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Short-lived — just long enough for the user to read and enter a code.
        var expires = DateTime.UtcNow.AddMinutes(5);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer, audience: _options.Audience, claims: claims,
            expires: expires, signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Guid? ValidateTwoFactorToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = true,
            }, out _);

            var type = principal.FindFirstValue("type");
            if (type != TwoFactorTokenType)
            {
                return null;
            }

            var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(sub, out var userId) ? userId : null;
        }
        catch
        {
            return null;
        }
    }
}
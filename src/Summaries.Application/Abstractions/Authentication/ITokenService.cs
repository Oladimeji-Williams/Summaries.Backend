namespace Summaries.Application.Abstractions.Authentication;

public interface ITokenService
{
    string GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions);

    Task<string> GenerateRefreshTokenAsync(
        Guid userId,
        CancellationToken cancellationToken);
}
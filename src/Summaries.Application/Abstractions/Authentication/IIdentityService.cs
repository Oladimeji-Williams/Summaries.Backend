using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.DTOs;

namespace Summaries.Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(
        string firstName, string lastName, string email, string password,
        CancellationToken cancellationToken);

    Task<AuthenticationResult?> LoginAsync(
        string email, string password, CancellationToken cancellationToken);

    Task<AuthenticationResult?> RefreshTokenAsync(
        string refreshToken, CancellationToken cancellationToken);

    Task<bool> RevokeRefreshTokenAsync(
        string refreshToken, CancellationToken cancellationToken);

    Task<UserProfileDto?> GetProfileAsync(
        Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserProfileDto>> GetAllUsersAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, UserProfileDto>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds, CancellationToken cancellationToken);

    Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken);

    Task<Result> ResetPasswordAsync(
        string email, string token, string newPassword, CancellationToken cancellationToken);

    Task<Result> ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken);

    Task<Result> UpdateProfileAsync(
        Guid userId, string firstName, string lastName, CancellationToken cancellationToken);
}
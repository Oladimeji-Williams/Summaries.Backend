using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.DTOs;

namespace Summaries.Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(
        string firstName, string lastName, string email, string password,
        CancellationToken cancellationToken);

    Task<LoginOutcome> LoginAsync(string email, string password, CancellationToken cancellationToken);

    Task<AuthenticationResult?> RefreshTokenAsync(
        string refreshToken, CancellationToken cancellationToken);

    Task<bool> RevokeRefreshTokenAsync(
        string refreshToken, CancellationToken cancellationToken);

    Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserProfileDto>> GetAllUsersAsync(CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, UserProfileDto>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds, CancellationToken cancellationToken);

    Task<Result> UpdateProfileAsync(
        Guid userId, string firstName, string lastName, string? phoneNumber,
        string? address, string? city, string? country, CancellationToken cancellationToken);

    Task<Result> UpdateAvatarAsync(Guid userId, string avatarUrl, CancellationToken cancellationToken);

    Task<Result> RemoveAvatarAsync(Guid userId, CancellationToken cancellationToken);

    Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken);

    Task<Result> ResetPasswordAsync(
        string email, string token, string newPassword, CancellationToken cancellationToken);

    Task<Result> ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken);

    Task<string?> GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellationToken);

    Task<Result> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken);

    Task<DateTimeOffset?> GetLastLoginNotificationSentAtAsync(Guid userId, CancellationToken cancellationToken);

    Task RecordLoginNotificationSentAsync(
        Guid userId, DateTimeOffset sentAtUtc, CancellationToken cancellationToken);

    Task<Result<TwoFactorSetupResult>> BeginTwoFactorSetupAsync(Guid userId, CancellationToken cancellationToken);

    Task<Result> ConfirmTwoFactorSetupAsync(Guid userId, string code, CancellationToken cancellationToken);

    Task<Result> DisableTwoFactorAsync(Guid userId, string currentPassword, CancellationToken cancellationToken);

    Task<bool> IsTwoFactorEnabledAsync(Guid userId, CancellationToken cancellationToken);

    Task<LoginOutcome> VerifyTwoFactorCodeAsync(
        string twoFactorToken, string code, CancellationToken cancellationToken);

    Task<LoginOutcome> LoginWithExternalProviderAsync(
        string provider, string providerKey, string email, string displayName, CancellationToken cancellationToken);

    Task<LoginStartOutcome> StartLoginAsync(string email, CancellationToken cancellationToken);
    Task<LoginOutcome> CompleteEmailSignInWithCodeAsync(string email, string code, CancellationToken cancellationToken);
    Task<LoginOutcome> CompleteEmailSignInWithLinkAsync(string token, CancellationToken cancellationToken);
    Task<Result> EnableEmailSignInAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result> DisableEmailSignInAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> IsEmailSignInEnabledAsync(Guid userId, CancellationToken cancellationToken);
}
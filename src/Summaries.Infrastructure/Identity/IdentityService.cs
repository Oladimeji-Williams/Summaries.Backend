using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Storage;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.Errors;
using Summaries.Application.Features.Users.Shared.DTOs;
using Summaries.Application.Features.Users.Shared.Errors;
using Summaries.Infrastructure.Authentication;
using Summaries.Application.Abstractions.Email;


namespace Summaries.Infrastructure.Identity;
internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    ApplicationIdentityDbContext dbContext,
    IOptions<JwtOptions> jwtOptions,
    IFileStorageService fileStorage,
    IEmailSender emailSender,
    IOptions<Summaries.Application.Common.Options.FrontendOptions> frontendOptions)
    : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ApplicationIdentityDbContext _dbContext = dbContext;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IFileStorageService _fileStorage = fileStorage;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly string _frontendBaseUrl = frontendOptions.Value.BaseUrl;

    public async Task<Result<Guid>> RegisterAsync(
        string firstName, string lastName, string email, string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedEmail = email.Trim().ToLowerInvariant();

        var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existingUser is not null)
        {
            return Result<Guid>.Failure(AuthErrors.EmailAlreadyExists());
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = normalizedEmail,
            Email = normalizedEmail,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(x => x.Description));
            return Result<Guid>.Failure(AuthErrors.RegistrationFailed(errors));
        }

        await _userManager.AddToRoleAsync(user, "User");

        return Result<Guid>.Success(user.Id);
    }

    public async Task<LoginOutcome> LoginAsync(
        string email, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            return new LoginOutcome.InvalidCredentials();
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (signInResult.IsLockedOut)
        {
            return new LoginOutcome.AccountLockedOut(user.LockoutEnd);
        }
        if (!signInResult.Succeeded)
        {
            return new LoginOutcome.InvalidCredentials();
        }

        if (!user.EmailConfirmed)
        {
            return new LoginOutcome.EmailNotConfirmed();
        }

        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            var twoFactorToken = _tokenService.GenerateTwoFactorToken(user.Id);
            return new LoginOutcome.RequiresTwoFactor(twoFactorToken);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles, []);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        await StoreRefreshTokenAsync(user.Id, refreshToken, cancellationToken);
        var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        var result = new AuthenticationResult(
            user.Id, user.Email!, $"{user.FirstName} {user.LastName}".Trim(),
            accessToken, refreshToken, accessTokenExpiresAtUtc, refreshTokenExpiresAtUtc,
            roles.ToList(), user.AvatarUrl);

        return new LoginOutcome.Success(result);
    }

    public async Task<AuthenticationResult?> RefreshTokenAsync(
        string refreshToken, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tokenHash = RefreshTokenHasher.Hash(refreshToken);
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        if (storedToken is null || !storedToken.IsActive)
        {
            return null;
        }
        var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null)
        {
            return null;
        }
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles, []);
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        storedToken.RevokedAtUtc = DateTime.UtcNow;
        storedToken.ReplacedByTokenHash = RefreshTokenHasher.Hash(newRefreshToken);
        await StoreRefreshTokenAsync(user.Id, newRefreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);
        return new AuthenticationResult(
            user.Id, user.Email!, $"{user.FirstName} {user.LastName}".Trim(),
            accessToken, newRefreshToken, accessTokenExpiresAtUtc, refreshTokenExpiresAtUtc,
            roles.ToList(), user.AvatarUrl);
    }

    public async Task<bool> RevokeRefreshTokenAsync(
        string refreshToken, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tokenHash = RefreshTokenHasher.Hash(refreshToken);
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        if (storedToken is null || !storedToken.IsActive)
        {
            return false;
        }
        storedToken.RevokedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.ToDto();
    }

    public async Task<IReadOnlyList<UserProfileDto>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _userManager.Users
            .OrderBy(u => u.Email)
            .Select(u => u.ToDto())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, UserProfileDto>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var idList = userIds.Distinct().ToList();
        var users = await _userManager.Users
            .Where(u => idList.Contains(u.Id))
            .Select(u => u.ToDto())
            .ToListAsync(cancellationToken);
        return users.ToDictionary(u => u.Id);
    }

    public async Task<Result> UpdateProfileAsync(
        Guid userId, string firstName, string lastName, string? phoneNumber,
        string? address, string? city, string? country, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        user.FirstName = firstName.Trim();
        user.LastName = lastName.Trim();
        user.PhoneNumber = phoneNumber;
        user.Address = address;
        user.City = city;
        user.Country = country;
        user.UpdatedAtUtc = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(AuthErrors.RegistrationFailed(errors));
        }

        return Result.Success();
    }

    public async Task<Result> UpdateAvatarAsync(Guid userId, string avatarUrl, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }
        user.AvatarUrl = avatarUrl;
        user.UpdatedAtUtc = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(AuthErrors.RegistrationFailed(errors));
        }
        return Result.Success();
    }

    public async Task<Result> RemoveAvatarAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            await _fileStorage.DeleteAsync(user.AvatarUrl, cancellationToken);
        }
        user.AvatarUrl = null;
        user.UpdatedAtUtc = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(AuthErrors.RegistrationFailed(errors));
        }
        return Result.Success();
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            return null;
        }
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<Result> ResetPasswordAsync(
        string email, string token, string newPassword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            return Result.Failure(AuthErrors.PasswordResetFailed("Invalid token."));
        }
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(AuthErrors.PasswordResetFailed(errors));
        }
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(AuthErrors.ChangePasswordFailed("User not found."));
        }
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(AuthErrors.ChangePasswordFailed(errors));
        }
        return Result.Success();
    }

    public async Task<string?> GenerateEmailConfirmationTokenAsync(
        string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null || user.EmailConfirmed)
        {
            return null;
        }
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<Result> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            return Result.Failure(AuthErrors.EmailConfirmationFailed("Invalid or expired confirmation link."));
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return Result.Failure(AuthErrors.EmailConfirmationFailed("Invalid or expired confirmation link."));
        }
        return Result.Success();
    }

    public async Task<DateTimeOffset?> GetLastLoginNotificationSentAtAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.LastLoginNotificationSentAtUtc is { } value
            ? new DateTimeOffset(value, TimeSpan.Zero)
            : null;
    }

    public async Task RecordLoginNotificationSentAsync(
        Guid userId, DateTimeOffset sentAtUtc, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return;
        }
        user.LastLoginNotificationSentAtUtc = sentAtUtc.UtcDateTime;
        await _userManager.UpdateAsync(user);
    }

    private async Task StoreRefreshTokenAsync(
        Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = RefreshTokenHasher.Hash(refreshToken),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
        };
        await _dbContext.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<TwoFactorSetupResult>> BeginTwoFactorSetupAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result<TwoFactorSetupResult>.Failure(UserErrors.NotFound(userId));
        }

        var sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(sharedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var authenticatorUri = GenerateAuthenticatorUri(user.Email!, sharedKey!);
        return Result<TwoFactorSetupResult>.Success(new TwoFactorSetupResult(sharedKey!, authenticatorUri));
    }

    public async Task<Result> ConfirmTwoFactorSetupAsync(
        Guid userId, string code, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);

        if (!isValid)
        {
            return Result.Failure(AuthErrors.InvalidTwoFactorCode());
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        user.EmailSignInEnabled = false;   // <-- add this line
        await _userManager.UpdateAsync(user);   // <-- and this line
        return Result.Success();
    }

    public async Task<Result> DisableTwoFactorAsync(
        Guid userId, string currentPassword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, currentPassword);
        if (!passwordValid)
        {
            return Result.Failure(AuthErrors.ChangePasswordFailed("Incorrect password."));
        }

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);
        return Result.Success();
    }

    public async Task<bool> IsTwoFactorEnabledAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is not null && await _userManager.GetTwoFactorEnabledAsync(user);
    }

    public async Task<LoginOutcome> VerifyTwoFactorCodeAsync(
        string twoFactorToken, string code, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userId = _tokenService.ValidateTwoFactorToken(twoFactorToken);
        if (userId is null)
        {
            return new LoginOutcome.InvalidCredentials();
        }

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user is null)
        {
            return new LoginOutcome.InvalidCredentials();
        }

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);
        Console.WriteLine($"[2FA DEBUG] TOTP check for user {user.Email}, code '{code}' => {isValid}");
        if (!isValid)
        {
            return new LoginOutcome.InvalidCredentials();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles, []);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        await StoreRefreshTokenAsync(user.Id, refreshToken, cancellationToken);
        var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        var result = new AuthenticationResult(
            user.Id, user.Email!, $"{user.FirstName} {user.LastName}".Trim(),
            accessToken, refreshToken, accessTokenExpiresAtUtc, refreshTokenExpiresAtUtc,
            roles.ToList(), user.AvatarUrl);

        return new LoginOutcome.Success(result);
    }

    private static string GenerateAuthenticatorUri(string email, string sharedKey)
    {
        const string issuer = "Summaries";
        return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}" +
            $"?secret={sharedKey}&issuer={Uri.EscapeDataString(issuer)}&digits=6";
    }

    public async Task<LoginOutcome> LoginWithExternalProviderAsync(
        string provider, string providerKey, string email, string displayName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _userManager.FindByLoginAsync(provider, providerKey);
        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(normalizedEmail);
            if (user is null)
            {
                var (firstName, lastName) = SplitDisplayName(displayName);
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = normalizedEmail,
                    Email = normalizedEmail,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true, // the provider already verified this email
                    CreatedAtUtc = DateTime.UtcNow,
                };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return new LoginOutcome.InvalidCredentials();
                }
                await _userManager.AddToRoleAsync(user, "User");
            }

            var addLoginResult = await _userManager.AddLoginAsync(
                user, new UserLoginInfo(provider, providerKey, provider));
            if (!addLoginResult.Succeeded)
            {
                return new LoginOutcome.InvalidCredentials();
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles, []);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        await StoreRefreshTokenAsync(user.Id, refreshToken, cancellationToken);
        var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        var result = new AuthenticationResult(
            user.Id, user.Email!, $"{user.FirstName} {user.LastName}".Trim(),
            accessToken, refreshToken, accessTokenExpiresAtUtc, refreshTokenExpiresAtUtc,
            roles.ToList(), user.AvatarUrl);

        return new LoginOutcome.Success(result);
    }

    private static (string FirstName, string LastName) SplitDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName)) return ("New", "User");
        var parts = displayName.Trim().Split(' ', 2);
        return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], "");
    }

    public async Task<LoginStartOutcome> StartLoginAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            return new LoginStartOutcome.AccountNotFound();
        }

        if (!user.EmailSignInEnabled)
        {
            if (!await _userManager.HasPasswordAsync(user))
            {
                return new LoginStartOutcome.NoPasswordSet();
            }
            return new LoginStartOutcome.UsePassword();
        }

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');

        var attempt = new EmailSignInAttempt
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CodeHash = SecurityTokenHasher.Hash(code),
            TokenHash = SecurityTokenHasher.Hash(token),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
        };
        await _dbContext.EmailSignInAttempts.AddAsync(attempt, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var magicLink = $"{_frontendBaseUrl}/login/verify?token={Uri.EscapeDataString(token)}";
        var sentAt = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(1));

        try
        {
            await _emailSender.SendSignInCodeAsync(user.Email!, code, magicLink, sentAt, cancellationToken);
        }
        catch
        {
            _dbContext.EmailSignInAttempts.Remove(attempt);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new LoginStartOutcome.EmailDeliveryFailed();
        }

        return new LoginStartOutcome.EmailCodeSent();
    }

    public async Task<LoginOutcome> CompleteEmailSignInWithCodeAsync(
        string email, string code, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            return new LoginOutcome.InvalidCredentials();
        }

        var codeHash = SecurityTokenHasher.Hash(code.Trim());
        var attempt = await _dbContext.EmailSignInAttempts
            .Where(a => a.UserId == user.Id && a.ConsumedAtUtc == null)
            .OrderByDescending(a => a.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (attempt is null || attempt.IsExpired || attempt.FailedAttempts >= 5)
        {
            return new LoginOutcome.InvalidCredentials();
        }

        if (attempt.CodeHash != codeHash)
        {
            attempt.FailedAttempts++;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new LoginOutcome.InvalidCredentials();
        }

        attempt.ConsumedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LoginOutcome.Success(await BuildAuthenticationResultAsync(user, cancellationToken));
    }

    public async Task<LoginOutcome> CompleteEmailSignInWithLinkAsync(string token, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tokenHash = SecurityTokenHasher.Hash(token.Trim());

        var attempt = await _dbContext.EmailSignInAttempts
            .FirstOrDefaultAsync(a => a.TokenHash == tokenHash && a.ConsumedAtUtc == null, cancellationToken);

        if (attempt is null || attempt.IsExpired)
        {
            return new LoginOutcome.InvalidCredentials();
        }

        var user = await _userManager.FindByIdAsync(attempt.UserId.ToString());
        if (user is null)
        {
            return new LoginOutcome.InvalidCredentials();
        }

        attempt.ConsumedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LoginOutcome.Success(await BuildAuthenticationResultAsync(user, cancellationToken));
    }

    public async Task<Result> EnableEmailSignInAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        user.EmailSignInEnabled = true;
        await _userManager.SetTwoFactorEnabledAsync(user, false);
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(AuthErrors.RegistrationFailed(errors));
        }
        return Result.Success();
    }

    public async Task<Result> DisableEmailSignInAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        user.EmailSignInEnabled = false;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(AuthErrors.RegistrationFailed(errors));
        }
        return Result.Success();
    }

    public async Task<bool> IsEmailSignInEnabledAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.EmailSignInEnabled ?? false;
    }

    private async Task<AuthenticationResult> BuildAuthenticationResultAsync(
        ApplicationUser user, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles, []);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        await StoreRefreshTokenAsync(user.Id, refreshToken, cancellationToken);
        var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        return new AuthenticationResult(
            user.Id, user.Email!, $"{user.FirstName} {user.LastName}".Trim(),
            accessToken, refreshToken, accessTokenExpiresAtUtc, refreshTokenExpiresAtUtc,
            roles.ToList(), user.AvatarUrl);
    }
}
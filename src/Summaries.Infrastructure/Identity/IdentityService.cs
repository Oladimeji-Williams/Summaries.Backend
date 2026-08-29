using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.Errors;
using Summaries.Application.Features.Users.Shared.DTOs;
using Summaries.Application.Features.Users.Shared.Errors;
using Summaries.Infrastructure.Authentication;

namespace Summaries.Infrastructure.Identity;

internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    ApplicationIdentityDbContext dbContext,
    IOptions<JwtOptions> jwtOptions)
    : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ApplicationIdentityDbContext _dbContext = dbContext;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

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
            CreatedAtUtc = DateTime.UtcNow
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

    public async Task<AuthenticationResult?> LoginAsync(
        string email, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            return null;
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return null;
        }
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles, []);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        await StoreRefreshTokenAsync(user.Id, refreshToken, cancellationToken);
        var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);
        return new AuthenticationResult(
            user.Id, user.Email!, $"{user.FirstName} {user.LastName}",
            accessToken, refreshToken, accessTokenExpiresAtUtc, refreshTokenExpiresAtUtc,
            roles.ToList());
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
            user.Id, user.Email!, $"{user.FirstName} {user.LastName}",
            accessToken, newRefreshToken, accessTokenExpiresAtUtc, refreshTokenExpiresAtUtc,
            roles.ToList());
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

    public async Task<UserProfileDto?> GetProfileAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return null;
        }
        return new UserProfileDto(user.Id, user.Email!, user.FirstName, user.LastName, user.CreatedAtUtc);
    }

    public async Task<IReadOnlyList<UserProfileDto>> GetAllUsersAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _userManager.Users
            .OrderBy(u => u.Email)
            .Select(u => new UserProfileDto(u.Id, u.Email!, u.FirstName, u.LastName, u.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, UserProfileDto>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var idList = userIds.Distinct().ToList();
        var users = await _userManager.Users
            .Where(u => idList.Contains(u.Id))
            .Select(u => new UserProfileDto(u.Id, u.Email!, u.FirstName, u.LastName, u.CreatedAtUtc))
            .ToListAsync(cancellationToken);
        return users.ToDictionary(u => u.Id);
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
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays)
        };
        await _dbContext.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(
        string email, CancellationToken cancellationToken)
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
            // Same anti-enumeration reasoning as ForgotPasswordCommandHandler —
            // don't reveal whether the account exists.
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

    public async Task<Result> UpdateProfileAsync(
        Guid userId, string firstName, string lastName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        user.FirstName = firstName.Trim();
        user.LastName = lastName.Trim();
        user.UpdatedAtUtc = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(AuthErrors.RegistrationFailed(errors)); // reuse: generic "identity op failed" shape
        }

        return Result.Success();
    }
}
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Shared.Errors;

public static class AuthErrors
{
    public static Error EmailAlreadyExists() => new(
        "Auth.EmailAlreadyExists", "An account with this email already exists.", ErrorType.Conflict);
    public static Error InvalidCredentials() => new(
        "Auth.InvalidCredentials", "The email or password is incorrect.", ErrorType.Unauthorized);
    public static Error RefreshTokenInvalid() => new(
        "Auth.RefreshTokenInvalid", "The refresh token is invalid or has expired.", ErrorType.Unauthorized);
    public static Error RefreshTokenRevoked() => new(
        "Auth.RefreshTokenRevoked", "The refresh token has been revoked.", ErrorType.Unauthorized);
    public static Error RegistrationFailed(string details) => new(
        "Auth.RegistrationFailed", details, ErrorType.Validation);
    public static Error PasswordResetFailed(string details) => new(
        "Auth.PasswordResetFailed", details, ErrorType.Validation);
    public static Error ChangePasswordFailed(string details) => new(
        "Auth.ChangePasswordFailed", details, ErrorType.Validation);
    public static Error EmailNotConfirmed() => new(
        "Auth.EmailNotConfirmed", "Please verify your email address before logging in.", ErrorType.Forbidden);
    public static Error EmailConfirmationFailed(string details) => new(
        "Auth.EmailConfirmationFailed", details, ErrorType.Validation);
    public static Error TwoFactorRequired(string twoFactorToken) => new(
        "Auth.TwoFactorRequired", twoFactorToken, ErrorType.Forbidden);
    public static Error InvalidTwoFactorCode() => new(
        "Auth.InvalidTwoFactorCode", "The verification code is incorrect or has expired.", ErrorType.Unauthorized);
    public static Error TwoFactorSetupFailed(string details) => new(
        "Auth.TwoFactorSetupFailed", details, ErrorType.Validation);
    public static Error AccountLockedOut(DateTimeOffset? until) => new(
        "Auth.AccountLockedOut", $"Account is locked until {until?.ToString("yyyy-MM-dd HH:mm:ss") ?? "unknown"}.", ErrorType.Unauthorized);
    public static Error InvalidSignInCode() => new(
        "Auth.InvalidSignInCode", "That code or link is invalid or has expired.", ErrorType.Unauthorized);
}
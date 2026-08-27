using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Shared.Errors;

public static class AuthErrors
{
    public static Error EmailAlreadyExists() => new(
        "Auth.EmailAlreadyExists",
        "An account with this email already exists.",
        ErrorType.Conflict);

    public static Error InvalidCredentials() => new(
        "Auth.InvalidCredentials",
        "The email or password is incorrect.",
        ErrorType.Unauthorized);

    public static Error RefreshTokenInvalid() => new(
        "Auth.RefreshTokenInvalid",
        "The refresh token is invalid or has expired.",
        ErrorType.Unauthorized);

    public static Error RefreshTokenRevoked() => new(
        "Auth.RefreshTokenRevoked",
        "The refresh token has been revoked.",
        ErrorType.Unauthorized);

    public static Error RegistrationFailed(string details) => new(
        "Auth.RegistrationFailed",
        details,
        ErrorType.Validation);
}
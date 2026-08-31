namespace Summaries.Application.Features.Users.Shared.DTOs;

public sealed record UserProfileDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAtUtc,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? Country
);
using Microsoft.AspNetCore.Identity;

namespace Summaries.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public string? AvatarUrl { get; set; }
}
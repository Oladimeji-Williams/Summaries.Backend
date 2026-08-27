using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Summaries.Infrastructure.Authentication;

namespace Summaries.Infrastructure.Identity;

public sealed class ApplicationIdentityDbContext
    : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        Guid>
{
    public ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens =>
        Set<RefreshToken>();

    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();
        });

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationIdentityDbContext).Assembly);
    }
}
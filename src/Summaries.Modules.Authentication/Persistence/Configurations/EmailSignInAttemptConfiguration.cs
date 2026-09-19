using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Summaries.Modules.Authentication.Infrastructure;
using Summaries.Modules.Authentication.Infrastructure.Identity;
namespace Summaries.Modules.Authentication.Persistence.Configurations;

public sealed class EmailSignInAttemptConfiguration : IEntityTypeConfiguration<EmailSignInAttempt>
{
    public void Configure(EntityTypeBuilder<EmailSignInAttempt> builder)
    {
        builder.ToTable("EmailSignInAttempts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CodeHash).IsRequired().HasMaxLength(64);
        builder.Property(x => x.TokenHash).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => x.TokenHash);
        builder.HasIndex(x => new { x.UserId, x.ConsumedAtUtc });
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
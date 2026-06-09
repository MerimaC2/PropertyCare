using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Infrastructure.Database.Configurations.Identity;

public class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(RefreshTokenEntity.Constraints.TokenHashMaxLength);

        builder.Property(x => x.Fingerprint)
            .HasMaxLength(RefreshTokenEntity.Constraints.FingerprintMaxLength);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

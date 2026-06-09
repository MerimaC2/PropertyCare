using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Infrastructure.Database.Configurations.Identity;

public class AppUserEntityConfiguration : IEntityTypeConfiguration<AppUserEntity>
{
    public void Configure(EntityTypeBuilder<AppUserEntity> builder)
    {
        builder.ToTable("AppUsers");

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(AppUserEntity.Constraints.FirstNameMaxLength);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(AppUserEntity.Constraints.LastNameMaxLength);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(AppUserEntity.Constraints.EmailMaxLength);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(AppUserEntity.Constraints.PasswordHashMaxLength);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Ignore(x => x.FullName);

        builder.HasOne(x => x.Tenant)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

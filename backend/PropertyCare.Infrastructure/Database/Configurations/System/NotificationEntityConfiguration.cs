using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.System;

namespace PropertyCare.Infrastructure.Database.Configurations.System;

public class NotificationEntityConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.ToTable("Notifications");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(NotificationEntity.Constraints.TitleMaxLength);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(NotificationEntity.Constraints.MessageMaxLength);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

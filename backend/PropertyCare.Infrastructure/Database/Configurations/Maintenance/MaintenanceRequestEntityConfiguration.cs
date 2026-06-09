using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Infrastructure.Database.Configurations.Maintenance;

public class MaintenanceRequestEntityConfiguration : IEntityTypeConfiguration<MaintenanceRequestEntity>
{
    public void Configure(EntityTypeBuilder<MaintenanceRequestEntity> builder)
    {
        builder.ToTable("MaintenanceRequests");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(MaintenanceRequestEntity.Constraints.TitleMaxLength);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(MaintenanceRequestEntity.Constraints.DescriptionMaxLength);

        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Building)
            .WithMany()
            .HasForeignKey(x => x.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Asset)
            .WithMany()
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Priority)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.PriorityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

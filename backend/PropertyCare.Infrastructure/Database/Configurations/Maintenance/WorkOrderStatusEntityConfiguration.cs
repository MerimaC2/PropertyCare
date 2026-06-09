using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Infrastructure.Database.Configurations.Maintenance;

public class WorkOrderStatusEntityConfiguration : IEntityTypeConfiguration<WorkOrderStatusEntity>
{
    public void Configure(EntityTypeBuilder<WorkOrderStatusEntity> builder)
    {
        builder.ToTable("WorkOrderStatuses");

        builder.Property(x => x.Abrv)
            .IsRequired()
            .HasMaxLength(WorkOrderStatusEntity.Constraints.AbrvMaxLength);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(WorkOrderStatusEntity.Constraints.NameMaxLength);

        builder.HasIndex(x => new { x.TenantId, x.Abrv })
            .IsUnique();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

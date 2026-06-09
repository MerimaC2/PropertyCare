using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Infrastructure.Database.Configurations.Maintenance;

public class WorkLogEntityConfiguration : IEntityTypeConfiguration<WorkLogEntity>
{
    public void Configure(EntityTypeBuilder<WorkLogEntity> builder)
    {
        builder.ToTable("WorkLogs");

        builder.Property(x => x.Note)
            .IsRequired()
            .HasMaxLength(WorkLogEntity.Constraints.NoteMaxLength);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WorkOrder)
            .WithMany(x => x.WorkLogs)
            .HasForeignKey(x => x.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

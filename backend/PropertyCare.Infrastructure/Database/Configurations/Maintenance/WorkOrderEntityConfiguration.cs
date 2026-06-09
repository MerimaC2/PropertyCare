using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Infrastructure.Database.Configurations.Maintenance;

public class WorkOrderEntityConfiguration : IEntityTypeConfiguration<WorkOrderEntity>
{
    public void Configure(EntityTypeBuilder<WorkOrderEntity> builder)
    {
        builder.ToTable("WorkOrders");

        builder.Property(x => x.Note)
            .HasMaxLength(WorkOrderEntity.Constraints.NoteMaxLength);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.WorkOrders)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedToUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.WorkOrders)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

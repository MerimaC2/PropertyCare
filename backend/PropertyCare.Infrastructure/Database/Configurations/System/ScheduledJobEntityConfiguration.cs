using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.System;

namespace PropertyCare.Infrastructure.Database.Configurations.System;

public class ScheduledJobEntityConfiguration : IEntityTypeConfiguration<ScheduledJobEntity>
{
    public void Configure(EntityTypeBuilder<ScheduledJobEntity> builder)
    {
        builder.ToTable("ScheduledJobs");

        builder.Property(x => x.Schedule)
            .IsRequired()
            .HasMaxLength(ScheduledJobEntity.Constraints.ScheduleMaxLength);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.JobType)
            .WithMany(x => x.ScheduledJobs)
            .HasForeignKey(x => x.JobTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

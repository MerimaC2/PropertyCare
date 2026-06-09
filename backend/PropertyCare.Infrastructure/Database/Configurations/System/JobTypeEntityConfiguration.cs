using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.System;

namespace PropertyCare.Infrastructure.Database.Configurations.System;

public class JobTypeEntityConfiguration : IEntityTypeConfiguration<JobTypeEntity>
{
    public void Configure(EntityTypeBuilder<JobTypeEntity> builder)
    {
        builder.ToTable("JobTypes");

        builder.Property(x => x.Abrv)
            .IsRequired()
            .HasMaxLength(JobTypeEntity.Constraints.AbrvMaxLength);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(JobTypeEntity.Constraints.NameMaxLength);

        builder.HasIndex(x => new { x.TenantId, x.Abrv })
            .IsUnique();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

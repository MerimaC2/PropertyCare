using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Infrastructure.Database.Configurations.Maintenance;

public class RequestPriorityEntityConfiguration : IEntityTypeConfiguration<RequestPriorityEntity>
{
    public void Configure(EntityTypeBuilder<RequestPriorityEntity> builder)
    {
        builder.ToTable("RequestPriorities");

        builder.Property(x => x.Abrv)
            .IsRequired()
            .HasMaxLength(RequestPriorityEntity.Constraints.AbrvMaxLength);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(RequestPriorityEntity.Constraints.NameMaxLength);

        builder.HasIndex(x => new { x.TenantId, x.Abrv })
            .IsUnique();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

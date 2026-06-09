using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Infrastructure.Database.Configurations.Maintenance;

public class RequestStatusEntityConfiguration : IEntityTypeConfiguration<RequestStatusEntity>
{
    public void Configure(EntityTypeBuilder<RequestStatusEntity> builder)
    {
        builder.ToTable("RequestStatuses");

        builder.Property(x => x.Abrv)
            .IsRequired()
            .HasMaxLength(RequestStatusEntity.Constraints.AbrvMaxLength);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(RequestStatusEntity.Constraints.NameMaxLength);

        builder.HasIndex(x => new { x.TenantId, x.Abrv })
            .IsUnique();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

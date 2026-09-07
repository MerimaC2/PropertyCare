using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Infrastructure.Database.Configurations.Facilities;

public class BuildingEntityConfiguration : IEntityTypeConfiguration<BuildingEntity>
{
    public void Configure(EntityTypeBuilder<BuildingEntity> builder)
    {
        builder.ToTable("Buildings");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(BuildingEntity.Constraints.NameMaxLength);

        builder.Property(x => x.NameNormalized)
            .IsRequired()
            .HasMaxLength(BuildingEntity.Constraints.NameMaxLength);

        // Names are unique within a tenant. Filtered on IsDeleted so a soft-deleted building does
        // not keep its name reserved forever.
        builder.HasIndex(x => new { x.TenantId, x.NameNormalized })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Buildings_TenantId_NameNormalized");

        builder.Property(x => x.Address)
            .HasMaxLength(BuildingEntity.Constraints.AddressMaxLength);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BuildingType)
            .WithMany(x => x.Buildings)
            .HasForeignKey(x => x.BuildingTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

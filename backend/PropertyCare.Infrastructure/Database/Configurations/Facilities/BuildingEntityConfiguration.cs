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

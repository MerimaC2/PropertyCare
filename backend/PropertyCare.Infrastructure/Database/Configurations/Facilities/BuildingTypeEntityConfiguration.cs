using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Infrastructure.Database.Configurations.Facilities;

public class BuildingTypeEntityConfiguration : IEntityTypeConfiguration<BuildingTypeEntity>
{
    public void Configure(EntityTypeBuilder<BuildingTypeEntity> builder)
    {
        builder.ToTable("BuildingTypes");

        builder.Property(x => x.Abrv)
            .IsRequired()
            .HasMaxLength(BuildingTypeEntity.Constraints.AbrvMaxLength);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(BuildingTypeEntity.Constraints.NameMaxLength);

        builder.HasIndex(x => new { x.TenantId, x.Abrv })
            .IsUnique();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

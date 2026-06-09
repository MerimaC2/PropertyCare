using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Infrastructure.Database.Configurations.Facilities;

public class AssetTypeEntityConfiguration : IEntityTypeConfiguration<AssetTypeEntity>
{
    public void Configure(EntityTypeBuilder<AssetTypeEntity> builder)
    {
        builder.ToTable("AssetTypes");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(AssetTypeEntity.Constraints.NameMaxLength);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

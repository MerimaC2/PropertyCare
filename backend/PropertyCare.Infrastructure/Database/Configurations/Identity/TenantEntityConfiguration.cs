using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Infrastructure.Database.Configurations.Identity;

public class TenantEntityConfiguration : IEntityTypeConfiguration<TenantEntity>
{
    public void Configure(EntityTypeBuilder<TenantEntity> builder)
    {
        builder.ToTable("Tenants");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(TenantEntity.Constraints.NameMaxLength);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Infrastructure.Database.Configurations.Maintenance;

public class RequestImageEntityConfiguration : IEntityTypeConfiguration<RequestImageEntity>
{
    public void Configure(EntityTypeBuilder<RequestImageEntity> builder)
    {
        builder.ToTable("RequestImages");

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(RequestImageEntity.Constraints.FileNameMaxLength);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(RequestImageEntity.Constraints.ContentTypeMaxLength);

        builder.Property(x => x.RelativePath)
            .IsRequired()
            .HasMaxLength(RequestImageEntity.Constraints.RelativePathMaxLength);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

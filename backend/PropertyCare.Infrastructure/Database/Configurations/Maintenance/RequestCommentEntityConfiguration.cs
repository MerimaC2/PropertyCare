using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Infrastructure.Database.Configurations.Maintenance;

public class RequestCommentEntityConfiguration : IEntityTypeConfiguration<RequestCommentEntity>
{
    public void Configure(EntityTypeBuilder<RequestCommentEntity> builder)
    {
        builder.ToTable("RequestComments");

        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(RequestCommentEntity.Constraints.TextMaxLength);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Infrastructure.Database.Configurations.Maintenance;

public class RequestStatusHistoryEntityConfiguration : IEntityTypeConfiguration<RequestStatusHistoryEntity>
{
    public void Configure(EntityTypeBuilder<RequestStatusHistoryEntity> builder)
    {
        builder.ToTable("RequestStatusHistories");

        builder.Property(x => x.Note)
            .HasMaxLength(RequestStatusHistoryEntity.Constraints.NoteMaxLength);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.StatusHistory)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FromStatus)
            .WithMany()
            .HasForeignKey(x => x.FromStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToStatus)
            .WithMany()
            .HasForeignKey(x => x.ToStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ChangedByUser)
            .WithMany()
            .HasForeignKey(x => x.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

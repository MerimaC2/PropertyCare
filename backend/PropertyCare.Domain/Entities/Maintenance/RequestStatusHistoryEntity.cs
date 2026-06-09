using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Maintenance;

/// <summary>Audit of request status transitions (who changed what and when).</summary>
public sealed class RequestStatusHistoryEntity : BaseEntity
{
    public int RequestId { get; set; }
    public MaintenanceRequestEntity Request { get; set; } = null!;

    public int? FromStatusId { get; set; }
    public RequestStatusEntity? FromStatus { get; set; }

    public int ToStatusId { get; set; }
    public RequestStatusEntity ToStatus { get; set; } = null!;

    public int ChangedByUserId { get; set; }
    public AppUserEntity ChangedByUser { get; set; } = null!;

    public string? Note { get; set; }

    public static class Constraints
    {
        public const int NoteMaxLength = 500;
    }
}

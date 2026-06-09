using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Maintenance;

/// <summary>An intervention assigned to a technician for a maintenance request.</summary>
public sealed class WorkOrderEntity : BaseEntity
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int RequestId { get; set; }
    public MaintenanceRequestEntity Request { get; set; } = null!;

    public int AssignedToUserId { get; set; }
    public AppUserEntity AssignedToUser { get; set; } = null!;

    public int StatusId { get; set; }
    public WorkOrderStatusEntity Status { get; set; } = null!;

    public string? Note { get; set; }

    public ICollection<WorkLogEntity> WorkLogs { get; set; } = new List<WorkLogEntity>();

    public static class Constraints
    {
        public const int NoteMaxLength = 1000;
    }
}

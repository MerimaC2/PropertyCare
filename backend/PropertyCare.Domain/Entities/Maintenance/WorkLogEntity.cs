using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Maintenance;

/// <summary>A short technician note with the time spent on a work order.</summary>
public sealed class WorkLogEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int WorkOrderId { get; set; }
    public WorkOrderEntity WorkOrder { get; set; } = null!;

    public string Note { get; set; } = null!;
    public int MinutesSpent { get; set; }

    public static class Constraints
    {
        public const int NoteMaxLength = 1000;
    }
}

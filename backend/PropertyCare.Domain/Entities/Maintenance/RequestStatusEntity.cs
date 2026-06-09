using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Maintenance;

public sealed class RequestStatusEntity : BaseEntity
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public string Abrv { get; set; } = null!;
    public string Name { get; set; } = null!;

    /// <summary>Terminal statuses close the request lifecycle (completed, cancelled).</summary>
    public bool IsTerminal { get; set; }

    public ICollection<MaintenanceRequestEntity> Requests { get; set; } = new List<MaintenanceRequestEntity>();

    public static class Constraints
    {
        public const int AbrvMaxLength = 30;
        public const int NameMaxLength = 60;
    }

    /// <summary>Well-known status abbreviations used by handlers.</summary>
    public static class Codes
    {
        public const string New = "NEW";
        public const string Assigned = "ASSIGNED";
        public const string InProgress = "IN_PROGRESS";
        public const string OnHold = "ON_HOLD";
        public const string Completed = "COMPLETED";
        public const string Cancelled = "CANCELLED";
    }
}

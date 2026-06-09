using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Maintenance;

public sealed class RequestPriorityEntity : BaseEntity
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public string Abrv { get; set; } = null!;
    public string Name { get; set; } = null!;

    /// <summary>Service-level agreement in hours: how fast a request with this priority should be resolved.</summary>
    public int SlaHours { get; set; }

    public ICollection<MaintenanceRequestEntity> Requests { get; set; } = new List<MaintenanceRequestEntity>();

    public static class Constraints
    {
        public const int AbrvMaxLength = 20;
        public const int NameMaxLength = 60;
    }
}

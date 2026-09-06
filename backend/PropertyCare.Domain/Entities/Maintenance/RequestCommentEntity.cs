using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Maintenance;

public sealed class RequestCommentEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int RequestId { get; set; }
    public MaintenanceRequestEntity Request { get; set; } = null!;

    public int UserId { get; set; }
    public AppUserEntity User { get; set; } = null!;

    public string Text { get; set; } = null!;

    public static class Constraints
    {
        public const int TextMaxLength = 1000;
    }
}

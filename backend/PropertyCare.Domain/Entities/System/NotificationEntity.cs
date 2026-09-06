using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.System;

public sealed class NotificationEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int UserId { get; set; }
    public AppUserEntity User { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }
    public NotificationType Type { get; set; } = NotificationType.General;

    public static class Constraints
    {
        public const int TitleMaxLength = 150;
        public const int MessageMaxLength = 500;
    }
}

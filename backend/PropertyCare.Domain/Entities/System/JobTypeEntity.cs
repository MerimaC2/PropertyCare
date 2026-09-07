using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.System;

public sealed class JobTypeEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public string Abrv { get; set; } = null!;
    public string Name { get; set; } = null!;

    public ICollection<ScheduledJobEntity> ScheduledJobs { get; set; } = new List<ScheduledJobEntity>();

    public static class Constraints
    {
        public const int AbrvMaxLength = 80;
        public const int NameMaxLength = 80;
    }
}

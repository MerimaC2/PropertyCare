using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.System;

/// <summary>A recurring job definition (e.g. periodic inspection), scheduled with a cron-like expression.</summary>
public sealed class ScheduledJobEntity : BaseEntity
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int JobTypeId { get; set; }
    public JobTypeEntity JobType { get; set; } = null!;

    public string Schedule { get; set; } = null!;

    public static class Constraints
    {
        public const int ScheduleMaxLength = 120;
    }
}

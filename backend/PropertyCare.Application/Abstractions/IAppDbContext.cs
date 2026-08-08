using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Domain.Entities.Identity;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Domain.Entities.System;

namespace PropertyCare.Application.Abstractions;

/// <summary>
/// Database access contract so Application handlers do not depend on the concrete EF context.
/// </summary>
public interface IAppDbContext
{
    DbSet<TenantEntity> Tenants { get; }
    DbSet<UserRoleEntity> UserRoles { get; }
    DbSet<AppUserEntity> Users { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }

    DbSet<BuildingTypeEntity> BuildingTypes { get; }
    DbSet<BuildingEntity> Buildings { get; }
    DbSet<UnitEntity> Units { get; }
    DbSet<AssetTypeEntity> AssetTypes { get; }
    DbSet<AssetEntity> Assets { get; }

    DbSet<RequestPriorityEntity> RequestPriorities { get; }
    DbSet<RequestStatusEntity> RequestStatuses { get; }
    DbSet<MaintenanceRequestEntity> MaintenanceRequests { get; }
    DbSet<RequestImageEntity> RequestImages { get; }
    DbSet<RequestCommentEntity> RequestComments { get; }
    DbSet<RequestStatusHistoryEntity> RequestStatusHistories { get; }
    DbSet<WorkOrderStatusEntity> WorkOrderStatuses { get; }
    DbSet<WorkOrderEntity> WorkOrders { get; }
    DbSet<WorkLogEntity> WorkLogs { get; }

    DbSet<NotificationEntity> Notifications { get; }
    DbSet<JobTypeEntity> JobTypes { get; }
    DbSet<ScheduledJobEntity> ScheduledJobs { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}

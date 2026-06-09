using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Abstractions;
using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Domain.Entities.Identity;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Domain.Entities.System;
using PropertyCare.Infrastructure.Database.Seeders;

namespace PropertyCare.Infrastructure.Database;

public class DatabaseContext : DbContext, IAppDbContext
{
    private readonly TimeProvider _clock;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, TimeProvider clock)
        : base(options)
    {
        _clock = clock;
    }

    public DbSet<TenantEntity> Tenants => Set<TenantEntity>();
    public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();
    public DbSet<AppUserEntity> Users => Set<AppUserEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    public DbSet<BuildingTypeEntity> BuildingTypes => Set<BuildingTypeEntity>();
    public DbSet<BuildingEntity> Buildings => Set<BuildingEntity>();
    public DbSet<UnitEntity> Units => Set<UnitEntity>();
    public DbSet<AssetTypeEntity> AssetTypes => Set<AssetTypeEntity>();
    public DbSet<AssetEntity> Assets => Set<AssetEntity>();

    public DbSet<RequestPriorityEntity> RequestPriorities => Set<RequestPriorityEntity>();
    public DbSet<RequestStatusEntity> RequestStatuses => Set<RequestStatusEntity>();
    public DbSet<MaintenanceRequestEntity> MaintenanceRequests => Set<MaintenanceRequestEntity>();
    public DbSet<RequestCommentEntity> RequestComments => Set<RequestCommentEntity>();
    public DbSet<RequestStatusHistoryEntity> RequestStatusHistories => Set<RequestStatusHistoryEntity>();
    public DbSet<WorkOrderStatusEntity> WorkOrderStatuses => Set<WorkOrderStatusEntity>();
    public DbSet<WorkOrderEntity> WorkOrders => Set<WorkOrderEntity>();
    public DbSet<WorkLogEntity> WorkLogs => Set<WorkLogEntity>();

    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();
    public DbSet<JobTypeEntity> JobTypes => Set<JobTypeEntity>();
    public DbSet<ScheduledJobEntity> ScheduledJobs => Set<ScheduledJobEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API configuration, one class per entity.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        // Static lookup data baked into migrations.
        StaticDataSeeder.Seed(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        StampAuditFields();
        return await base.SaveChangesAsync(ct);
    }

    Task<int> IAppDbContext.SaveChangesAsync(CancellationToken ct)
        => SaveChangesAsync(ct);

    private void StampAuditFields()
    {
        var nowUtc = _clock.GetUtcNow().UtcDateTime;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case Microsoft.EntityFrameworkCore.EntityState.Added when entry.Entity.CreatedAtUtc == default:
                    entry.Entity.CreatedAtUtc = nowUtc;
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Modified:
                    entry.Entity.ModifiedAtUtc = nowUtc;
                    break;
            }
        }
    }
}

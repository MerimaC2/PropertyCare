using System.Linq.Expressions;
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
    private readonly IAppCurrentUser _currentUser;

    public DatabaseContext(
        DbContextOptions<DatabaseContext> options,
        TimeProvider clock,
        IAppCurrentUser currentUser)
        : base(options)
    {
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Tenant of the signed-in user, read fresh on every query. It is null when nobody is signed
    /// in - seeding, migrations, login and refresh - and then the tenant filters match no rows at
    /// all. Those few paths ask for <c>IgnoreQueryFilters()</c> explicitly, so the filter stays
    /// closed by default instead of open by default.
    /// </summary>
    public int? CurrentTenantId => _currentUser.TenantId;

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
    public DbSet<RequestImageEntity> RequestImages => Set<RequestImageEntity>();
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

        ApplyTenantFilters(modelBuilder);

        // Static lookup data baked into migrations.
        StaticDataSeeder.Seed(modelBuilder);
    }

    /// <summary>
    /// Gives every <see cref="ITenantScoped"/> entity the same filter: TenantId must equal the
    /// tenant of the signed-in user. Written once here rather than repeated in each handler,
    /// because a filter that has to be remembered is a filter that will be forgotten.
    /// </summary>
    private void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(ITenantScoped).IsAssignableFrom(t.ClrType)))
        {
            // e => (int?)e.TenantId == this.CurrentTenantId
            var entity = Expression.Parameter(entityType.ClrType, "e");
            var body = Expression.Equal(
                Expression.Convert(
                    Expression.Property(entity, nameof(ITenantScoped.TenantId)),
                    typeof(int?)),
                Expression.Property(
                    Expression.Constant(this),
                    nameof(CurrentTenantId)));

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(Expression.Lambda(body, entity));
        }

        // The tenant row itself is scoped by its own key.
        modelBuilder.Entity<TenantEntity>().HasQueryFilter(t => t.Id == CurrentTenantId);

        // These three carry no TenantId of their own - they belong wherever their parent belongs.
        // Matching the parent's filter keeps a query that starts at the child (image content, for
        // instance) just as closed as one that starts at the request.
        modelBuilder.Entity<RequestImageEntity>()
            .HasQueryFilter(i => i.Request.TenantId == CurrentTenantId);
        modelBuilder.Entity<RequestStatusHistoryEntity>()
            .HasQueryFilter(h => h.Request.TenantId == CurrentTenantId);
        modelBuilder.Entity<RefreshTokenEntity>()
            .HasQueryFilter(t => t.User.TenantId == CurrentTenantId);
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

using Microsoft.EntityFrameworkCore;
using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Domain.Entities.Identity;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Domain.Entities.System;

namespace PropertyCare.Infrastructure.Database.Seeders;

/// <summary>
/// Static lookup data baked into migrations through HasData.
/// Values must stay stable so migrations remain deterministic.
/// </summary>
public static class StaticDataSeeder
{
    private static readonly DateTime SeedDateUtc = new(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc);

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedTenants(modelBuilder);
        SeedUserRoles(modelBuilder);
        SeedRequestStatuses(modelBuilder);
        SeedWorkOrderStatuses(modelBuilder);
        SeedRequestPriorities(modelBuilder);
        SeedBuildingTypes(modelBuilder);
        SeedAssetTypes(modelBuilder);
        SeedJobTypes(modelBuilder);
    }

    private static void SeedTenants(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantEntity>().HasData(
            new TenantEntity { Id = 1, Name = "PropertyCare", CreatedAtUtc = SeedDateUtc });
    }

    private static void SeedUserRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRoleEntity>().HasData(
            new UserRoleEntity { Id = 1, TenantId = 1, Abrv = "ADMIN", Name = UserRoleEntity.Names.Administrator, CreatedAtUtc = SeedDateUtc },
            new UserRoleEntity { Id = 2, TenantId = 1, Abrv = "TECH", Name = UserRoleEntity.Names.Technician, CreatedAtUtc = SeedDateUtc },
            new UserRoleEntity { Id = 3, TenantId = 1, Abrv = "REP", Name = UserRoleEntity.Names.Reporter, CreatedAtUtc = SeedDateUtc });
    }

    private static void SeedRequestStatuses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RequestStatusEntity>().HasData(
            new RequestStatusEntity { Id = 1, TenantId = 1, Abrv = RequestStatusEntity.Codes.New, Name = "New", IsTerminal = false, CreatedAtUtc = SeedDateUtc },
            new RequestStatusEntity { Id = 2, TenantId = 1, Abrv = RequestStatusEntity.Codes.Assigned, Name = "Assigned", IsTerminal = false, CreatedAtUtc = SeedDateUtc },
            new RequestStatusEntity { Id = 3, TenantId = 1, Abrv = RequestStatusEntity.Codes.InProgress, Name = "In progress", IsTerminal = false, CreatedAtUtc = SeedDateUtc },
            new RequestStatusEntity { Id = 4, TenantId = 1, Abrv = RequestStatusEntity.Codes.OnHold, Name = "On hold", IsTerminal = false, CreatedAtUtc = SeedDateUtc },
            new RequestStatusEntity { Id = 5, TenantId = 1, Abrv = RequestStatusEntity.Codes.Completed, Name = "Completed", IsTerminal = true, CreatedAtUtc = SeedDateUtc },
            new RequestStatusEntity { Id = 6, TenantId = 1, Abrv = RequestStatusEntity.Codes.Cancelled, Name = "Cancelled", IsTerminal = true, CreatedAtUtc = SeedDateUtc });
    }

    private static void SeedWorkOrderStatuses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkOrderStatusEntity>().HasData(
            new WorkOrderStatusEntity { Id = 1, TenantId = 1, Abrv = WorkOrderStatusEntity.Codes.Assigned, Name = "Assigned", IsTerminal = false, CreatedAtUtc = SeedDateUtc },
            new WorkOrderStatusEntity { Id = 2, TenantId = 1, Abrv = WorkOrderStatusEntity.Codes.InProgress, Name = "In progress", IsTerminal = false, CreatedAtUtc = SeedDateUtc },
            new WorkOrderStatusEntity { Id = 3, TenantId = 1, Abrv = WorkOrderStatusEntity.Codes.OnHold, Name = "On hold", IsTerminal = false, CreatedAtUtc = SeedDateUtc },
            new WorkOrderStatusEntity { Id = 4, TenantId = 1, Abrv = WorkOrderStatusEntity.Codes.Completed, Name = "Completed", IsTerminal = true, CreatedAtUtc = SeedDateUtc },
            new WorkOrderStatusEntity { Id = 5, TenantId = 1, Abrv = WorkOrderStatusEntity.Codes.Cancelled, Name = "Cancelled", IsTerminal = true, CreatedAtUtc = SeedDateUtc });
    }

    private static void SeedRequestPriorities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RequestPriorityEntity>().HasData(
            new RequestPriorityEntity { Id = 1, TenantId = 1, Abrv = "LOW", Name = "Low", SlaHours = 72, CreatedAtUtc = SeedDateUtc },
            new RequestPriorityEntity { Id = 2, TenantId = 1, Abrv = "MED", Name = "Medium", SlaHours = 48, CreatedAtUtc = SeedDateUtc },
            new RequestPriorityEntity { Id = 3, TenantId = 1, Abrv = "HIGH", Name = "High", SlaHours = 24, CreatedAtUtc = SeedDateUtc },
            new RequestPriorityEntity { Id = 4, TenantId = 1, Abrv = "CRIT", Name = "Critical", SlaHours = 4, CreatedAtUtc = SeedDateUtc });
    }

    private static void SeedBuildingTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuildingTypeEntity>().HasData(
            new BuildingTypeEntity { Id = 1, TenantId = 1, Abrv = "OFFICE", Name = "Office building", CreatedAtUtc = SeedDateUtc },
            new BuildingTypeEntity { Id = 2, TenantId = 1, Abrv = "RES", Name = "Residential building", CreatedAtUtc = SeedDateUtc },
            new BuildingTypeEntity { Id = 3, TenantId = 1, Abrv = "WH", Name = "Warehouse", CreatedAtUtc = SeedDateUtc });
    }

    private static void SeedAssetTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssetTypeEntity>().HasData(
            new AssetTypeEntity { Id = 1, TenantId = 1, Name = "HVAC", DefaultSlaHours = 24, CreatedAtUtc = SeedDateUtc },
            new AssetTypeEntity { Id = 2, TenantId = 1, Name = "Elevator", DefaultSlaHours = 8, CreatedAtUtc = SeedDateUtc },
            new AssetTypeEntity { Id = 3, TenantId = 1, Name = "Plumbing", DefaultSlaHours = 48, CreatedAtUtc = SeedDateUtc },
            new AssetTypeEntity { Id = 4, TenantId = 1, Name = "Electrical", DefaultSlaHours = 24, CreatedAtUtc = SeedDateUtc },
            new AssetTypeEntity { Id = 5, TenantId = 1, Name = "Appliance", DefaultSlaHours = 72, CreatedAtUtc = SeedDateUtc });
    }

    private static void SeedJobTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobTypeEntity>().HasData(
            new JobTypeEntity { Id = 1, TenantId = 1, Abrv = "INSPECT", Name = "Inspection", CreatedAtUtc = SeedDateUtc },
            new JobTypeEntity { Id = 2, TenantId = 1, Abrv = "PREVENT", Name = "Preventive maintenance", CreatedAtUtc = SeedDateUtc });
    }
}

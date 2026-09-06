using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Domain.Entities.Identity;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Domain.Entities.System;

namespace PropertyCare.Infrastructure.Database.Seeders;

/// <summary>
/// Runtime demo data: users with hashed passwords, buildings, units, assets,
/// maintenance requests and work orders, so every screen has data after a fresh migration.
///
/// Demo logins:
///   admin@propertycare.ba       / Admin123!
///   technician1@propertycare.ba / Tech123!
///   technician2@propertycare.ba / Tech123!
///   reporter1@propertycare.ba   / Reporter123!
///   reporter2@propertycare.ba   / Reporter123!
///
/// Second tenant (Stanogradnja d.o.o.), seeded so tenant isolation can be seen working:
///   admin@stanogradnja.ba      / Admin123!
///   technician@stanogradnja.ba / Tech123!
///   reporter@stanogradnja.ba   / Reporter123!
/// </summary>
public class DynamicDataSeeder : IDatabaseSeeder
{
    private const int TenantId = 1;
    private const string SecondTenantName = "Stanogradnja d.o.o.";

    private readonly DatabaseContext _ctx;
    private readonly IPasswordHasher<AppUserEntity> _hasher;
    private readonly TimeProvider _clock;
    private AppUserEntity _admin = null!;

    public DynamicDataSeeder(
        DatabaseContext ctx,
        IPasswordHasher<AppUserEntity> hasher,
        TimeProvider clock)
    {
        _ctx = ctx;
        _hasher = hasher;
        _clock = clock;
    }

    public async Task SeedDynamicDataAsync(CancellationToken ct = default)
    {
        var nowUtc = _clock.GetUtcNow().UtcDateTime;

        // Seeding runs with nobody signed in, so every read here has to look past the global
        // tenant filter. Each tenant is checked on its own, so a development database that
        // already holds the first tenant still picks up the second one without being dropped.
        if (!await _ctx.Users.IgnoreQueryFilters().AnyAsync(ct))
            await SeedFirstTenantAsync(nowUtc, ct);

        if (!await _ctx.Tenants.IgnoreQueryFilters().AnyAsync(t => t.Name == SecondTenantName, ct))
            await SeedSecondTenantAsync(nowUtc, ct);
    }

    private async Task SeedFirstTenantAsync(DateTime nowUtc, CancellationToken ct)
    {
        // --- Users (role ids come from StaticDataSeeder: 1 admin, 2 technician, 3 reporter) ---
        var admin = _admin = CreateUser(1, "Selma", "Hodžić", "admin@propertycare.ba", "Admin123!", nowUtc.AddDays(-40));
        var technician1 = CreateUser(2, "Emir", "Kovač", "technician1@propertycare.ba", "Tech123!", nowUtc.AddDays(-38));
        var technician2 = CreateUser(2, "Tarik", "Begić", "technician2@propertycare.ba", "Tech123!", nowUtc.AddDays(-38));
        var reporter1 = CreateUser(3, "Lejla", "Mehić", "reporter1@propertycare.ba", "Reporter123!", nowUtc.AddDays(-35));
        var reporter2 = CreateUser(3, "Amar", "Softić", "reporter2@propertycare.ba", "Reporter123!", nowUtc.AddDays(-35));
        _ctx.Users.AddRange(admin, technician1, technician2, reporter1, reporter2);

        // --- Buildings (building type ids: 1 office, 2 residential, 3 warehouse) ---
        var alpha = new BuildingEntity { TenantId = TenantId, BuildingTypeId = 1, Name = "Alpha Business Center", NameNormalized = BuildingEntity.NormalizeName("Alpha Business Center"), Address = "Zmaja od Bosne 12, Sarajevo", CreatedAtUtc = nowUtc.AddDays(-34) };
        var park = new BuildingEntity { TenantId = TenantId, BuildingTypeId = 2, Name = "Park Residence", NameNormalized = BuildingEntity.NormalizeName("Park Residence"), Address = "Maršala Tita 45, Mostar", CreatedAtUtc = nowUtc.AddDays(-34) };
        var hub = new BuildingEntity { TenantId = TenantId, BuildingTypeId = 3, Name = "Logistics Hub East", NameNormalized = BuildingEntity.NormalizeName("Logistics Hub East"), Address = "Industrijska zona bb, Tuzla", CreatedAtUtc = nowUtc.AddDays(-34) };
        _ctx.Buildings.AddRange(alpha, park, hub);

        // --- Units ---
        var office101 = new UnitEntity { TenantId = TenantId, Building = alpha, Label = "Office 101", CreatedAtUtc = nowUtc.AddDays(-33) };
        var office202 = new UnitEntity { TenantId = TenantId, Building = alpha, Label = "Office 202", CreatedAtUtc = nowUtc.AddDays(-33) };
        var confRoomA = new UnitEntity { TenantId = TenantId, Building = alpha, Label = "Conference Room A", CreatedAtUtc = nowUtc.AddDays(-33) };
        var lobby = new UnitEntity { TenantId = TenantId, Building = park, Label = "Lobby", CreatedAtUtc = nowUtc.AddDays(-33) };
        var apartment7A = new UnitEntity { TenantId = TenantId, Building = park, Label = "Apartment 7A", CreatedAtUtc = nowUtc.AddDays(-33) };
        var storageHall = new UnitEntity { TenantId = TenantId, Building = hub, Label = "Storage Hall 1", CreatedAtUtc = nowUtc.AddDays(-33) };
        _ctx.Units.AddRange(office101, office202, confRoomA, lobby, apartment7A, storageHall);

        // --- Assets (asset type ids: 1 HVAC, 2 elevator, 3 plumbing, 4 electrical, 5 appliance) ---
        var acUnit = new AssetEntity { TenantId = TenantId, Unit = office101, AssetTypeId = 1, Name = "Air conditioner AC-X500", CreatedAtUtc = nowUtc.AddDays(-32) };
        var projector = new AssetEntity { TenantId = TenantId, Unit = confRoomA, AssetTypeId = 4, Name = "Projector Epson EB-2250", CreatedAtUtc = nowUtc.AddDays(-32) };
        var elevator = new AssetEntity { TenantId = TenantId, Unit = lobby, AssetTypeId = 2, Name = "Passenger elevator E1", CreatedAtUtc = nowUtc.AddDays(-32) };
        var waterHeater = new AssetEntity { TenantId = TenantId, Unit = apartment7A, AssetTypeId = 3, Name = "Water heater 80L", CreatedAtUtc = nowUtc.AddDays(-32) };
        var charger = new AssetEntity { TenantId = TenantId, Unit = storageHall, AssetTypeId = 4, Name = "Forklift charging station", CreatedAtUtc = nowUtc.AddDays(-32) };
        var coffeeMachine = new AssetEntity { TenantId = TenantId, Unit = office202, AssetTypeId = 5, Name = "Coffee machine Jura X8", CreatedAtUtc = nowUtc.AddDays(-32) };
        _ctx.Assets.AddRange(acUnit, projector, elevator, waterHeater, charger, coffeeMachine);

        // --- Maintenance requests ---
        // Status ids: 1 NEW, 2 ASSIGNED, 3 IN_PROGRESS, 4 ON_HOLD, 5 COMPLETED, 6 CANCELLED
        // Priority ids: 1 LOW, 2 MED, 3 HIGH, 4 CRIT

        AddRequest(reporter1, alpha, office101, acUnit, priorityId: 3, statusId: 1, nowUtc.AddDays(-2),
            "Air conditioner is leaking", "Water is dripping from the AC unit in office 101 and forming a puddle on the floor.");

        AddRequest(reporter1, alpha, confRoomA, projector, priorityId: 2, statusId: 1, nowUtc.AddDays(-1),
            "Projector does not turn on", "The projector in Conference Room A shows no signal light when plugged in.");

        AddRequest(reporter2, hub, storageHall, charger, priorityId: 4, statusId: 1, nowUtc.AddHours(-5),
            "Charging station sparks", "The forklift charging station sparks when a vehicle is connected. Potential fire hazard.");

        var assigned1 = AddRequest(reporter1, park, lobby, elevator, priorityId: 4, statusId: 2, nowUtc.AddDays(-6),
            "Elevator stuck between floors", "The passenger elevator stopped between the 2nd and 3rd floor. Nobody is inside, but it does not respond.");
        AddWorkOrder(assigned1, technician1, statusId: 1, nowUtc.AddDays(-5), "Inspect the elevator control unit first.");

        var assigned2 = AddRequest(reporter2, park, apartment7A, waterHeater, priorityId: 2, statusId: 2, nowUtc.AddDays(-4),
            "Water heater leaking from valve", "The safety valve of the water heater drips constantly even when the heater is off.");
        AddWorkOrder(assigned2, technician2, statusId: 1, nowUtc.AddDays(-3), null);

        var inProgress = AddRequest(reporter1, alpha, office202, coffeeMachine, priorityId: 1, statusId: 3, nowUtc.AddDays(-10),
            "Coffee machine error E8", "The coffee machine shows error E8 and refuses to brew coffee. The team is desperate.");
        AddWorkOrder(inProgress, technician1, statusId: 2, nowUtc.AddDays(-9), "Descaling in progress, waiting for parts.");

        var onHold = AddRequest(reporter2, hub, storageHall, null, priorityId: 1, statusId: 4, nowUtc.AddDays(-15),
            "Broken window in storage hall", "A window pane in Storage Hall 1 is cracked, cold air comes in during the night shift.");
        AddWorkOrder(onHold, technician2, statusId: 3, nowUtc.AddDays(-14), "Waiting for the replacement glass delivery.");

        var completed = AddRequest(reporter1, alpha, office101, null, priorityId: 2, statusId: 5, nowUtc.AddDays(-20),
            "Flickering light in hallway", "The hallway light in front of office 101 flickers constantly.");
        var completedWo = AddWorkOrder(completed, technician1, statusId: 4, nowUtc.AddDays(-19), "Replaced the faulty ballast and tube.");
        _ctx.WorkLogs.Add(new WorkLogEntity
        {
            TenantId = TenantId,
            WorkOrder = completedWo,
            Note = "Replaced ballast and fluorescent tube, tested for 30 minutes.",
            MinutesSpent = 45,
            CreatedAtUtc = nowUtc.AddDays(-19).AddHours(3)
        });

        // --- A few notifications (mixed types and read/unread, so the notification center has data to filter) ---
        // The administrator and the technicians get one too: the notification center is open to
        // every role now, and an empty screen would say nothing about whether it works.
        _ctx.Notifications.AddRange(
            new NotificationEntity
            {
                TenantId = TenantId,
                User = admin,
                Type = NotificationType.General,
                Title = "Welcome to PropertyCare",
                Message = "New requests land in Triage. Assign them to a technician from there.",
                IsRead = false,
                CreatedAtUtc = nowUtc.AddDays(-40)
            },
            new NotificationEntity
            {
                TenantId = TenantId,
                User = technician1,
                Type = NotificationType.General,
                Title = "Welcome to PropertyCare",
                Message = "Work assigned to you shows up under My interventions.",
                IsRead = false,
                CreatedAtUtc = nowUtc.AddDays(-38)
            },
            new NotificationEntity
            {
                TenantId = TenantId,
                User = technician2,
                Type = NotificationType.General,
                Title = "Welcome to PropertyCare",
                Message = "Work assigned to you shows up under My interventions.",
                IsRead = true,
                CreatedAtUtc = nowUtc.AddDays(-38)
            },
            new NotificationEntity
            {
                TenantId = TenantId,
                User = reporter1,
                Type = NotificationType.General,
                Title = "Welcome to PropertyCare",
                Message = "You can report a fault any time and track its progress here.",
                IsRead = true,
                CreatedAtUtc = nowUtc.AddDays(-35)
            },
            new NotificationEntity
            {
                TenantId = TenantId,
                User = reporter1,
                Type = NotificationType.RequestSubmitted,
                Title = "Request submitted",
                Message = "Your request 'Air conditioner is leaking' was submitted and is awaiting triage.",
                IsRead = true,
                CreatedAtUtc = nowUtc.AddDays(-2)
            },
            new NotificationEntity
            {
                TenantId = TenantId,
                User = reporter1,
                Type = NotificationType.Assigned,
                Title = "Request assigned",
                Message = "Your request 'Elevator stuck between floors' was assigned to a technician.",
                IsRead = false,
                CreatedAtUtc = nowUtc.AddDays(-5)
            },
            new NotificationEntity
            {
                TenantId = TenantId,
                User = reporter1,
                Type = NotificationType.Completed,
                Title = "Request completed",
                Message = "Your request 'Flickering light in hallway' was completed.",
                IsRead = true,
                CreatedAtUtc = nowUtc.AddDays(-19)
            },
            new NotificationEntity
            {
                TenantId = TenantId,
                User = reporter2,
                Type = NotificationType.RequestSubmitted,
                Title = "Request submitted",
                Message = "Your request 'Charging station sparks' was submitted and is awaiting triage.",
                IsRead = false,
                CreatedAtUtc = nowUtc.AddHours(-5)
            },
            new NotificationEntity
            {
                TenantId = TenantId,
                User = reporter2,
                Type = NotificationType.Assigned,
                Title = "Request assigned",
                Message = "Your request 'Water heater leaking from valve' was assigned to a technician.",
                IsRead = false,
                CreatedAtUtc = nowUtc.AddDays(-3)
            });

        // --- Scheduled jobs (job type ids: 1 inspection, 2 preventive) ---
        _ctx.ScheduledJobs.AddRange(
            new ScheduledJobEntity { TenantId = TenantId, JobTypeId = 1, Schedule = "0 6 * * 1", CreatedAtUtc = nowUtc.AddDays(-30) },
            new ScheduledJobEntity { TenantId = TenantId, JobTypeId = 2, Schedule = "0 7 1 * *", CreatedAtUtc = nowUtc.AddDays(-30) });

        await _ctx.SaveChangesAsync(ct);
    }

    /// <summary>
    /// A second organisation with its own lookups, users, buildings and requests. With a single
    /// tenant in the seed the tenant filter cannot be observed at all - signing in here is what
    /// makes the isolation visible during manual testing.
    /// </summary>
    private async Task SeedSecondTenantAsync(DateTime nowUtc, CancellationToken ct)
    {
        var createdAtUtc = nowUtc.AddDays(-25);

        var tenant = new TenantEntity { Name = SecondTenantName, CreatedAtUtc = createdAtUtc };
        _ctx.Tenants.Add(tenant);
        await _ctx.SaveChangesAsync(ct);

        // Lookups carry a TenantId in this model, so the second tenant needs its own copies of
        // what StaticDataSeeder created for tenant 1. Copying keeps the codes (NEW, ASSIGNED...)
        // identical, which is what the handlers look up by.
        var roles = await CopyForTenantAsync(
            _ctx.UserRoles,
            r => new UserRoleEntity
            {
                TenantId = tenant.Id, Abrv = r.Abrv, Name = r.Name, CreatedAtUtc = createdAtUtc
            },
            ct);

        var requestStatuses = await CopyForTenantAsync(
            _ctx.RequestStatuses,
            s => new RequestStatusEntity
            {
                TenantId = tenant.Id, Abrv = s.Abrv, Name = s.Name,
                IsTerminal = s.IsTerminal, CreatedAtUtc = createdAtUtc
            },
            ct);

        var workOrderStatuses = await CopyForTenantAsync(
            _ctx.WorkOrderStatuses,
            s => new WorkOrderStatusEntity
            {
                TenantId = tenant.Id, Abrv = s.Abrv, Name = s.Name,
                IsTerminal = s.IsTerminal, CreatedAtUtc = createdAtUtc
            },
            ct);

        var priorities = await CopyForTenantAsync(
            _ctx.RequestPriorities,
            p => new RequestPriorityEntity
            {
                TenantId = tenant.Id, Abrv = p.Abrv, Name = p.Name,
                SlaHours = p.SlaHours, CreatedAtUtc = createdAtUtc
            },
            ct);

        var buildingTypes = await CopyForTenantAsync(
            _ctx.BuildingTypes,
            t => new BuildingTypeEntity
            {
                TenantId = tenant.Id, Abrv = t.Abrv, Name = t.Name, CreatedAtUtc = createdAtUtc
            },
            ct);

        var assetTypes = await CopyForTenantAsync(
            _ctx.AssetTypes,
            t => new AssetTypeEntity
            {
                TenantId = tenant.Id, Name = t.Name,
                DefaultSlaHours = t.DefaultSlaHours, CreatedAtUtc = createdAtUtc
            },
            ct);

        var admin = CreateUser(tenant.Id, roles, UserRoleEntity.Names.Administrator,
            "Nedim", "Alispahić", "admin@stanogradnja.ba", "Admin123!", createdAtUtc);
        var technician = CreateUser(tenant.Id, roles, UserRoleEntity.Names.Technician,
            "Vedad", "Šarić", "technician@stanogradnja.ba", "Tech123!", createdAtUtc);
        var reporter = CreateUser(tenant.Id, roles, UserRoleEntity.Names.Reporter,
            "Ajla", "Delić", "reporter@stanogradnja.ba", "Reporter123!", createdAtUtc);
        _ctx.Users.AddRange(admin, technician, reporter);

        // Deliberately the same name as a building of tenant 1: the unique index is per tenant,
        // so this must be allowed, and neither tenant may see the other's copy.
        var alpha = NewBuilding(tenant.Id, buildingTypes, "OFFICE",
            "Alpha Business Center", "Titova 8, Sarajevo", createdAtUtc);
        var vila = NewBuilding(tenant.Id, buildingTypes, "RES",
            "Vila Neretva", "Kralja Tvrtka 3, Mostar", createdAtUtc);
        _ctx.Buildings.AddRange(alpha, vila);

        var floor2 = new UnitEntity
        {
            TenantId = tenant.Id, Building = alpha, Label = "Floor 2", CreatedAtUtc = createdAtUtc
        };
        var apartment3 = new UnitEntity
        {
            TenantId = tenant.Id, Building = vila, Label = "Apartment 3", CreatedAtUtc = createdAtUtc
        };
        _ctx.Units.AddRange(floor2, apartment3);

        var boiler = new AssetEntity
        {
            TenantId = tenant.Id,
            Unit = apartment3,
            AssetType = assetTypes.Single(t => t.Name == "Plumbing"),
            Name = "Gas boiler Vaillant",
            CreatedAtUtc = createdAtUtc
        };
        _ctx.Assets.Add(boiler);

        var newRequest = new MaintenanceRequestEntity
        {
            TenantId = tenant.Id,
            Building = vila,
            Unit = apartment3,
            Asset = boiler,
            CreatedByUser = reporter,
            Priority = priorities.Single(p => p.Abrv == "HIGH"),
            Status = requestStatuses.Single(s => s.Abrv == RequestStatusEntity.Codes.New),
            Title = "Boiler makes a loud noise",
            Description = "The gas boiler in apartment 3 rattles loudly every time it starts.",
            CreatedAtUtc = nowUtc.AddDays(-3)
        };
        newRequest.StatusHistory.Add(new RequestStatusHistoryEntity
        {
            ToStatus = requestStatuses.Single(s => s.Abrv == RequestStatusEntity.Codes.New),
            ChangedByUser = reporter,
            Note = "Request created.",
            CreatedAtUtc = nowUtc.AddDays(-3)
        });

        var assignedRequest = new MaintenanceRequestEntity
        {
            TenantId = tenant.Id,
            Building = alpha,
            Unit = floor2,
            CreatedByUser = reporter,
            Priority = priorities.Single(p => p.Abrv == "MED"),
            Status = requestStatuses.Single(s => s.Abrv == RequestStatusEntity.Codes.Assigned),
            Title = "Entrance door does not lock",
            Description = "The main entrance door on floor 2 does not lock after office hours.",
            CreatedAtUtc = nowUtc.AddDays(-7)
        };
        assignedRequest.StatusHistory.Add(new RequestStatusHistoryEntity
        {
            ToStatus = requestStatuses.Single(s => s.Abrv == RequestStatusEntity.Codes.New),
            ChangedByUser = reporter,
            Note = "Request created.",
            CreatedAtUtc = nowUtc.AddDays(-7)
        });
        _ctx.MaintenanceRequests.AddRange(newRequest, assignedRequest);

        _ctx.WorkOrders.Add(new WorkOrderEntity
        {
            TenantId = tenant.Id,
            Request = assignedRequest,
            AssignedToUser = technician,
            Status = workOrderStatuses.Single(s => s.Abrv == WorkOrderStatusEntity.Codes.Assigned),
            Note = "Check the lock cylinder and the closer.",
            CreatedAtUtc = nowUtc.AddDays(-6)
        });

        _ctx.Notifications.AddRange(
            new NotificationEntity
            {
                TenantId = tenant.Id,
                User = reporter,
                Type = NotificationType.RequestSubmitted,
                Title = "Request submitted",
                Message = "Your request 'Boiler makes a loud noise' was submitted and is awaiting triage.",
                IsRead = false,
                CreatedAtUtc = nowUtc.AddDays(-3)
            },
            new NotificationEntity
            {
                TenantId = tenant.Id,
                User = admin,
                Type = NotificationType.General,
                Title = "Welcome to PropertyCare",
                Message = "New requests land in Triage. Assign them to a technician from there.",
                IsRead = false,
                CreatedAtUtc = createdAtUtc
            },
            new NotificationEntity
            {
                TenantId = tenant.Id,
                User = technician,
                Type = NotificationType.General,
                Title = "Welcome to PropertyCare",
                Message = "Work assigned to you shows up under My interventions.",
                IsRead = false,
                CreatedAtUtc = createdAtUtc
            });

        await _ctx.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Reads tenant 1's rows of a lookup table past the tenant filter and returns unsaved copies
    /// for the new tenant.
    /// </summary>
    private async Task<List<TEntity>> CopyForTenantAsync<TEntity>(
        DbSet<TEntity> set,
        Func<TEntity, TEntity> copy,
        CancellationToken ct)
        where TEntity : class, ITenantScoped
    {
        var copies = (await set.IgnoreQueryFilters().AsNoTracking()
                .Where(e => e.TenantId == TenantId)
                .OrderBy(e => EF.Property<int>(e, "Id"))
                .ToListAsync(ct))
            .Select(copy)
            .ToList();

        set.AddRange(copies);
        return copies;
    }

    private AppUserEntity CreateUser(
        int tenantId,
        List<UserRoleEntity> roles,
        string roleName,
        string firstName,
        string lastName,
        string email,
        string password,
        DateTime createdAtUtc)
    {
        var user = new AppUserEntity
        {
            TenantId = tenantId,
            Role = roles.Single(r => r.Name == roleName),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            IsActive = true,
            CreatedAtUtc = createdAtUtc,
            PasswordHash = string.Empty
        };
        user.PasswordHash = _hasher.HashPassword(user, password);
        return user;
    }

    private static BuildingEntity NewBuilding(
        int tenantId,
        List<BuildingTypeEntity> types,
        string typeAbrv,
        string name,
        string address,
        DateTime createdAtUtc)
        => new()
        {
            TenantId = tenantId,
            BuildingType = types.Single(t => t.Abrv == typeAbrv),
            Name = name,
            NameNormalized = BuildingEntity.NormalizeName(name),
            Address = address,
            CreatedAtUtc = createdAtUtc
        };

    private AppUserEntity CreateUser(
        int roleId, string firstName, string lastName, string email, string password, DateTime createdAtUtc)
    {
        var user = new AppUserEntity
        {
            TenantId = TenantId,
            RoleId = roleId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            IsActive = true,
            CreatedAtUtc = createdAtUtc,
            PasswordHash = string.Empty
        };
        user.PasswordHash = _hasher.HashPassword(user, password);
        return user;
    }

    private MaintenanceRequestEntity AddRequest(
        AppUserEntity createdBy,
        BuildingEntity building,
        UnitEntity? unit,
        AssetEntity? asset,
        int priorityId,
        int statusId,
        DateTime createdAtUtc,
        string title,
        string description)
    {
        var request = new MaintenanceRequestEntity
        {
            TenantId = TenantId,
            Building = building,
            Unit = unit,
            Asset = asset,
            CreatedByUser = createdBy,
            PriorityId = priorityId,
            StatusId = statusId,
            Title = title,
            Description = description,
            CreatedAtUtc = createdAtUtc
        };

        // Initial transition into NEW.
        request.StatusHistory.Add(new RequestStatusHistoryEntity
        {
            FromStatusId = null,
            ToStatusId = 1,
            ChangedByUser = createdBy,
            Note = "Request created.",
            CreatedAtUtc = createdAtUtc
        });

        // When the request is seeded past NEW, record the transition as well.
        if (statusId != 1)
        {
            request.StatusHistory.Add(new RequestStatusHistoryEntity
            {
                FromStatusId = 1,
                ToStatusId = statusId,
                ChangedByUser = _admin,
                Note = "Status updated during triage.",
                CreatedAtUtc = createdAtUtc.AddHours(8)
            });
        }

        _ctx.MaintenanceRequests.Add(request);
        return request;
    }

    private WorkOrderEntity AddWorkOrder(
        MaintenanceRequestEntity request,
        AppUserEntity technician,
        int statusId,
        DateTime createdAtUtc,
        string? note)
    {
        var workOrder = new WorkOrderEntity
        {
            TenantId = TenantId,
            Request = request,
            AssignedToUser = technician,
            StatusId = statusId,
            Note = note,
            CreatedAtUtc = createdAtUtc
        };
        _ctx.WorkOrders.Add(workOrder);
        return workOrder;
    }
}

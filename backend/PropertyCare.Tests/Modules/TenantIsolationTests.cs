using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Facilities.Buildings.Queries.List;
using PropertyCare.Application.Modules.Lookups.Queries.GetRequestFormLookups;
using PropertyCare.Application.Modules.Lookups.Queries.GetTriageLookups;
using PropertyCare.Application.Modules.MaintenanceRequests.Commands.Create;
using PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListForTriage;
using PropertyCare.Application.Modules.WorkOrders.Commands.Assign;
using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

/// <summary>
/// Two tenants over one database. The review asked for exactly this: proof that tenant 1 can
/// neither see nor reference the data of tenant 2.
/// </summary>
public class TenantIsolationTests
{
    private const int TenantOne = 1;
    private const int TenantTwo = 2;

    [Fact]
    public void GlobalFilter_HidesTheOtherTenantsRows_FromEachSide()
    {
        var databaseName = TestDbContextFactory.NewDatabaseName();
        using var asTenantOne = TestDbContextFactory.Create(TenantOne, databaseName);
        using var asTenantTwo = TestDbContextFactory.Create(TenantTwo, databaseName);

        TestData.AddBuilding(asTenantOne, "Alpha Business Center", TenantOne);
        TestData.AddBuilding(asTenantTwo, "Vila Neretva", TenantTwo);

        // Both contexts read the same store and each sees only its own row. Were the tenant baked
        // into the cached model, one of these two lists would hold both buildings.
        Assert.Equal(["Alpha Business Center"], asTenantOne.Buildings.Select(b => b.Name).ToList());
        Assert.Equal(["Vila Neretva"], asTenantTwo.Buildings.Select(b => b.Name).ToList());
    }

    [Fact]
    public async Task ListBuildings_ReturnsOnlyTheOwnTenantsBuildings()
    {
        var databaseName = TestDbContextFactory.NewDatabaseName();
        using var asTenantOne = TestDbContextFactory.Create(TenantOne, databaseName);
        using var asTenantTwo = TestDbContextFactory.Create(TenantTwo, databaseName);

        TestData.AddBuilding(asTenantOne, "Alpha Business Center", TenantOne);
        AddTenantTwoBuilding(asTenantTwo, "Vila Neretva");

        var handler = new ListBuildingsQueryHandler(
            asTenantOne, new FakeCurrentUser { UserId = 1, TenantId = TenantOne });

        var page = await handler.Handle(
            new ListBuildingsQuery { Paging = new PageRequest() }, CancellationToken.None);

        Assert.Single(page.Items);
        Assert.Equal("Alpha Business Center", page.Items[0].Name);
    }

    [Fact]
    public async Task CreateMaintenanceRequest_PointingAtAnotherTenantsBuilding_IsRejected()
    {
        var databaseName = TestDbContextFactory.NewDatabaseName();
        using var asTenantOne = TestDbContextFactory.Create(TenantOne, databaseName);
        using var asTenantTwo = TestDbContextFactory.Create(TenantTwo, databaseName);

        var reporter = TestData.AddUser(asTenantOne, roleId: 3, email: "reporter@test.ba");
        var foreignBuilding = AddTenantTwoBuilding(asTenantTwo, "Vila Neretva");

        var handler = new CreateMaintenanceRequestCommandHandler(
            asTenantOne, new FakeCurrentUser { UserId = reporter.Id, TenantId = TenantOne });

        // This is the hole the review described: the id existed, so the check passed and the
        // request ended up with a foreign key into another tenant.
        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(
                new CreateMaintenanceRequestCommand
                {
                    Title = "Broken lock",
                    Description = "The lock on the main door is broken.",
                    BuildingId = foreignBuilding.Id,
                    PriorityId = 2
                },
                CancellationToken.None));

        Assert.Empty(await asTenantOne.MaintenanceRequests.IgnoreQueryFilters().ToListAsync());
    }

    [Fact]
    public async Task AssignWorkOrder_ToATechnicianOfAnotherTenant_IsRejected()
    {
        var databaseName = TestDbContextFactory.NewDatabaseName();
        using var asTenantOne = TestDbContextFactory.Create(TenantOne, databaseName);
        using var asTenantTwo = TestDbContextFactory.Create(TenantTwo, databaseName);

        var admin = TestData.AddUser(asTenantOne, roleId: 1, email: "admin@test.ba");
        var reporter = TestData.AddUser(asTenantOne, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(asTenantOne, "Alpha Business Center", TenantOne);
        var request = AddRequest(asTenantOne, TenantOne, building.Id, reporter.Id);

        var foreignTechnician = TestData.AddUser(
            asTenantTwo, roleId: 2, email: "tech@other.ba", tenantId: TenantTwo);

        var handler = new AssignWorkOrderCommandHandler(
            asTenantOne, new FakeCurrentUser { UserId = admin.Id, TenantId = TenantOne });

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(
                new AssignWorkOrderCommand
                {
                    RequestId = request.Id,
                    AssignedToUserId = foreignTechnician.Id
                },
                CancellationToken.None));

        Assert.Empty(await asTenantOne.WorkOrders.IgnoreQueryFilters().ToListAsync());
    }

    [Fact]
    public async Task AssignWorkOrder_OnAnotherTenantsRequest_IsRejected()
    {
        var databaseName = TestDbContextFactory.NewDatabaseName();
        using var asTenantOne = TestDbContextFactory.Create(TenantOne, databaseName);
        using var asTenantTwo = TestDbContextFactory.Create(TenantTwo, databaseName);

        var admin = TestData.AddUser(asTenantOne, roleId: 1, email: "admin@test.ba");
        var foreignReporter = TestData.AddUser(
            asTenantTwo, roleId: 3, email: "reporter@other.ba", tenantId: TenantTwo);
        var foreignBuilding = AddTenantTwoBuilding(asTenantTwo, "Vila Neretva");
        var foreignRequest = AddRequest(
            asTenantTwo, TenantTwo, foreignBuilding.Id, foreignReporter.Id);

        var handler = new AssignWorkOrderCommandHandler(
            asTenantOne, new FakeCurrentUser { UserId = admin.Id, TenantId = TenantOne });

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(
                new AssignWorkOrderCommand
                {
                    RequestId = foreignRequest.Id,
                    AssignedToUserId = admin.Id
                },
                CancellationToken.None));
    }

    [Fact]
    public async Task TriageList_ShowsOnlyTheOwnTenantsRequests()
    {
        var databaseName = TestDbContextFactory.NewDatabaseName();
        using var asTenantOne = TestDbContextFactory.Create(TenantOne, databaseName);
        using var asTenantTwo = TestDbContextFactory.Create(TenantTwo, databaseName);

        var reporter = TestData.AddUser(asTenantOne, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(asTenantOne, "Alpha Business Center", TenantOne);
        AddRequest(asTenantOne, TenantOne, building.Id, reporter.Id, "Own request");

        var foreignReporter = TestData.AddUser(
            asTenantTwo, roleId: 3, email: "reporter@other.ba", tenantId: TenantTwo);
        var foreignBuilding = AddTenantTwoBuilding(asTenantTwo, "Vila Neretva");
        AddRequest(asTenantTwo, TenantTwo, foreignBuilding.Id, foreignReporter.Id, "Foreign request");

        var handler = new ListTriageRequestsQueryHandler(
            asTenantOne, new FakeCurrentUser { UserId = 1, TenantId = TenantOne });

        var page = await handler.Handle(
            new ListTriageRequestsQuery { Paging = new PageRequest() }, CancellationToken.None);

        Assert.Single(page.Items);
        Assert.Equal("Own request", page.Items[0].Title);
    }

    [Fact]
    public async Task RequestFormLookups_ShowNothingOfTheOtherTenant()
    {
        var databaseName = TestDbContextFactory.NewDatabaseName();
        using var asTenantOne = TestDbContextFactory.Create(TenantOne, databaseName);
        using var asTenantTwo = TestDbContextFactory.Create(TenantTwo, databaseName);

        TestData.AddBuilding(asTenantOne, "Alpha Business Center", TenantOne);
        AddTenantTwoBuilding(asTenantTwo, "Vila Neretva");

        var lookups = await new GetRequestFormLookupsQueryHandler(
                asTenantOne, new FakeCurrentUser { UserId = 1, TenantId = TenantOne })
            .Handle(new GetRequestFormLookupsQuery(), CancellationToken.None);

        Assert.Single(lookups.Buildings);
        Assert.Equal("Alpha Business Center", lookups.Buildings[0].Name);
    }

    [Fact]
    public async Task TriageLookups_OfferNoTechnicianOfTheOtherTenant()
    {
        var databaseName = TestDbContextFactory.NewDatabaseName();
        using var asTenantOne = TestDbContextFactory.Create(TenantOne, databaseName);
        using var asTenantTwo = TestDbContextFactory.Create(TenantTwo, databaseName);

        TestData.AddUser(asTenantOne, roleId: 2, email: "tech@test.ba");
        TestData.AddUser(asTenantTwo, roleId: 2, email: "tech@other.ba", tenantId: TenantTwo);

        var lookups = await new GetTriageLookupsQueryHandler(
                asTenantOne, new FakeCurrentUser { UserId = 1, TenantId = TenantOne })
            .Handle(new GetTriageLookupsQuery(), CancellationToken.None);

        Assert.Single(lookups.Technicians);
    }

    [Fact]
    public async Task ACallerWithoutATenant_GetsForbiddenInsteadOfAnEmptyList()
    {
        using var ctx = TestDbContextFactory.Create();

        var handler = new ListTriageRequestsQueryHandler(
            ctx, new FakeCurrentUser { UserId = 1, TenantId = null });

        await Assert.ThrowsAsync<ForbiddenException>(
            () => handler.Handle(
                new ListTriageRequestsQuery { Paging = new PageRequest() }, CancellationToken.None));
    }

    /// <summary>
    /// Tenant 2 has no static lookup rows of its own, so its building needs its own type.
    /// </summary>
    private static BuildingEntity AddTenantTwoBuilding(DatabaseContext ctx, string name)
    {
        var type = new BuildingTypeEntity
        {
            TenantId = TenantTwo,
            Abrv = "OFFICE",
            Name = "Office building"
        };
        ctx.BuildingTypes.Add(type);
        ctx.SaveChanges();

        var building = new BuildingEntity
        {
            TenantId = TenantTwo,
            BuildingTypeId = type.Id,
            Name = name,
            NameNormalized = BuildingEntity.NormalizeName(name)
        };
        ctx.Buildings.Add(building);
        ctx.SaveChanges();
        return building;
    }

    private static MaintenanceRequestEntity AddRequest(
        DatabaseContext ctx, int tenantId, int buildingId, int reporterId, string title = "Broken AC")
    {
        var status = new RequestStatusEntity
        {
            TenantId = tenantId,
            Abrv = RequestStatusEntity.Codes.New,
            Name = "New",
            IsTerminal = false
        };
        var priority = new RequestPriorityEntity
        {
            TenantId = tenantId,
            Abrv = "MED",
            Name = "Medium",
            SlaHours = 48
        };
        ctx.RequestStatuses.Add(status);
        ctx.RequestPriorities.Add(priority);
        ctx.SaveChanges();

        var request = new MaintenanceRequestEntity
        {
            TenantId = tenantId,
            BuildingId = buildingId,
            CreatedByUserId = reporterId,
            PriorityId = priority.Id,
            StatusId = status.Id,
            Title = title,
            Description = "Seeded for a tenant isolation test."
        };
        ctx.MaintenanceRequests.Add(request);
        ctx.SaveChanges();
        return request;
    }
}

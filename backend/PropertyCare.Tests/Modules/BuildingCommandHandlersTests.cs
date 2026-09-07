using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Facilities.Buildings.Commands.Create;
using PropertyCare.Application.Modules.Facilities.Buildings.Commands.Delete;
using PropertyCare.Application.Modules.Facilities.Buildings.Commands.Update;
using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class BuildingCommandHandlersTests
{
    [Fact]
    public async Task Create_ValidCommand_CreatesBuilding()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var handler = new CreateBuildingCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        var id = await handler.Handle(new CreateBuildingCommand
        {
            Name = "New Tower",
            Address = "Main Street 1",
            BuildingTypeId = 1, // office (static seed)
            Latitude = 43.85,
            Longitude = 18.41
        }, CancellationToken.None);

        var saved = await ctx.Buildings.SingleAsync(b => b.Id == id);
        Assert.Equal("New Tower", saved.Name);
        Assert.Equal(1, saved.BuildingTypeId);
        Assert.Equal(43.85, saved.Latitude);
        Assert.Equal(18.41, saved.Longitude);
    }

    [Fact]
    public async Task Create_UnknownBuildingType_ThrowsValidationException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var handler = new CreateBuildingCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(
                new CreateBuildingCommand { Name = "X", BuildingTypeId = 999 },
                CancellationToken.None));
    }

    [Fact]
    public async Task Create_NameAlreadyTakenIgnoringCaseAndSpacing_ThrowsConflictException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        TestData.AddBuilding(ctx, "Alpha Business Center");

        var handler = new CreateBuildingCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(
                new CreateBuildingCommand { Name = "  alpha BUSINESS center ", BuildingTypeId = 1 },
                CancellationToken.None));
    }

    [Fact]
    public async Task Create_NameTakenByAnotherTenant_IsAllowed()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        TestData.AddBuilding(ctx, "Shared Name", tenantId: 2);

        var handler = new CreateBuildingCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        var id = await handler.Handle(
            new CreateBuildingCommand { Name = "Shared Name", BuildingTypeId = 1 },
            CancellationToken.None);

        Assert.True(id > 0);
    }

    [Fact]
    public async Task Update_NameTakenByAnotherBuilding_ThrowsConflictException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        TestData.AddBuilding(ctx, "Park Residence");
        var edited = TestData.AddBuilding(ctx, "Logistics Hub East");

        var handler = new UpdateBuildingCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(
                new UpdateBuildingCommand
                {
                    Id = edited.Id,
                    Name = "park residence",
                    BuildingTypeId = 1
                },
                CancellationToken.None));
    }

    [Fact]
    public async Task Update_BuildingKeepsItsOwnName_Succeeds()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var building = TestData.AddBuilding(ctx, "Park Residence");

        var handler = new UpdateBuildingCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        await handler.Handle(
            new UpdateBuildingCommand
            {
                Id = building.Id,
                Name = "Park Residence",
                Address = "New Address 5",
                BuildingTypeId = 1
            },
            CancellationToken.None);

        var saved = await ctx.Buildings.SingleAsync(b => b.Id == building.Id);
        Assert.Equal("New Address 5", saved.Address);
    }

    [Fact]
    public async Task Create_StoresTheNormalizedNameAlongsideTheDisplayName()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var handler = new CreateBuildingCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        var id = await handler.Handle(
            new CreateBuildingCommand { Name = "  Riverside Lofts  ", BuildingTypeId = 1 },
            CancellationToken.None);

        var saved = await ctx.Buildings.SingleAsync(b => b.Id == id);
        Assert.Equal("Riverside Lofts", saved.Name);
        Assert.Equal("RIVERSIDE LOFTS", saved.NameNormalized);
    }

    [Fact]
    public async Task Delete_BuildingWithUnits_ThrowsConflictException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var building = TestData.AddBuilding(ctx);
        ctx.Units.Add(new UnitEntity { TenantId = 1, BuildingId = building.Id, Label = "Office 1" });
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteBuildingCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(
                new DeleteBuildingCommand { Id = building.Id },
                CancellationToken.None));
    }
}

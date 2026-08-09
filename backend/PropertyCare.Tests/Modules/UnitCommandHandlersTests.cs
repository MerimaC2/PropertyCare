using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Facilities.Units.Commands.Create;
using PropertyCare.Application.Modules.Facilities.Units.Commands.Delete;
using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class UnitCommandHandlersTests
{
    [Fact]
    public async Task Create_ValidCommand_CreatesUnit()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var building = TestData.AddBuilding(ctx);
        var handler = new CreateUnitCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        var id = await handler.Handle(
            new CreateUnitCommand { BuildingId = building.Id, Label = "Office 5" },
            CancellationToken.None);

        var saved = await ctx.Units.SingleAsync(u => u.Id == id);
        Assert.Equal("Office 5", saved.Label);
        Assert.Equal(building.Id, saved.BuildingId);
    }

    [Fact]
    public async Task Create_UnknownBuilding_ThrowsValidationException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var handler = new CreateUnitCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(
                new CreateUnitCommand { BuildingId = 9999, Label = "Office 5" },
                CancellationToken.None));
    }

    [Fact]
    public async Task Delete_UnitWithAssets_ThrowsConflictException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var building = TestData.AddBuilding(ctx);
        var unit = new UnitEntity { TenantId = 1, BuildingId = building.Id, Label = "Office 5" };
        ctx.Units.Add(unit);
        await ctx.SaveChangesAsync(CancellationToken.None);
        ctx.Assets.Add(new AssetEntity
        {
            TenantId = 1,
            UnitId = unit.Id,
            AssetTypeId = 1,
            Name = "AC unit"
        });
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteUnitCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new DeleteUnitCommand { Id = unit.Id }, CancellationToken.None));
    }
}

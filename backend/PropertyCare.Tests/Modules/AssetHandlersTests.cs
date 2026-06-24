using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Facilities.Assets.Commands.Create;
using PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Delete;
using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class AssetHandlersTests
{
    [Fact]
    public async Task CreateAsset_ValidCommand_CreatesAsset()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var building = TestData.AddBuilding(ctx);
        var unit = new UnitEntity { TenantId = 1, BuildingId = building.Id, Label = "Office 1" };
        ctx.Units.Add(unit);
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateAssetCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        var id = await handler.Handle(
            new CreateAssetCommand { UnitId = unit.Id, Name = "Boiler B-1", AssetTypeId = 1 },
            CancellationToken.None);

        var saved = await ctx.Assets.SingleAsync(a => a.Id == id);
        Assert.Equal("Boiler B-1", saved.Name);
        Assert.Equal(unit.Id, saved.UnitId);
        Assert.Equal(1, saved.AssetTypeId);
    }

    [Fact]
    public async Task CreateAsset_UnknownUnit_ThrowsValidationException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var handler = new CreateAssetCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(
                new CreateAssetCommand { UnitId = 9999, Name = "X", AssetTypeId = 1 },
                CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAssetType_InUse_ThrowsConflictException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var building = TestData.AddBuilding(ctx);
        var unit = new UnitEntity { TenantId = 1, BuildingId = building.Id, Label = "Office 1" };
        ctx.Units.Add(unit);
        await ctx.SaveChangesAsync(CancellationToken.None);
        ctx.Assets.Add(new AssetEntity
        {
            TenantId = 1,
            UnitId = unit.Id,
            AssetTypeId = 1, // static-seeded asset type
            Name = "AC unit"
        });
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteAssetTypeCommandHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new DeleteAssetTypeCommand { Id = 1 }, CancellationToken.None));
    }
}

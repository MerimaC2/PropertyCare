using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Facilities.Buildings.Commands.Create;
using PropertyCare.Application.Modules.Facilities.Buildings.Commands.Delete;
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

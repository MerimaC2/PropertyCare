using PropertyCare.Application.Modules.Facilities.Buildings.Queries.NameExists;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class BuildingNameExistsQueryHandlerTests
{
    [Fact]
    public async Task Returns_True_When_Name_Taken_By_Another_Building()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        TestData.AddBuilding(ctx, name: "Alpha Center");

        var handler = new BuildingNameExistsQueryHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        var exists = await handler.Handle(
            new BuildingNameExistsQuery { Name = "Alpha Center" }, CancellationToken.None);

        Assert.True(exists);
    }

    [Fact]
    public async Task Returns_False_When_Name_Belongs_To_Excluded_Building()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var building = TestData.AddBuilding(ctx, name: "Alpha Center");

        var handler = new BuildingNameExistsQueryHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        var exists = await handler.Handle(
            new BuildingNameExistsQuery { Name = "Alpha Center", ExcludeId = building.Id },
            CancellationToken.None);

        Assert.False(exists);
    }

    [Fact]
    public async Task Returns_False_When_Name_Free()
    {
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");

        var handler = new BuildingNameExistsQueryHandler(
            ctx, new FakeCurrentUser { UserId = admin.Id, TenantId = 1 });

        var exists = await handler.Handle(
            new BuildingNameExistsQuery { Name = "Brand New Building" }, CancellationToken.None);

        Assert.False(exists);
    }
}

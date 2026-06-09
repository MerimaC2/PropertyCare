using PropertyCare.Application.Common;
using PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListMy;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class ListMyMaintenanceRequestsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyOwnRequests_AndAppliesStatusFilter()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var me = TestData.AddUser(ctx, roleId: 3, email: "me@test.ba");
        var someoneElse = TestData.AddUser(ctx, roleId: 3, email: "other@test.ba");
        var building = TestData.AddBuilding(ctx);

        ctx.MaintenanceRequests.AddRange(
            NewRequest(me.Id, building.Id, statusId: 1, title: "My new request"),
            NewRequest(me.Id, building.Id, statusId: 5, title: "My completed request"),
            NewRequest(someoneElse.Id, building.Id, statusId: 1, title: "Foreign request"));
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new ListMyMaintenanceRequestsQueryHandler(
            ctx, new FakeCurrentUser { UserId = me.Id });

        // Act - no filters: only own requests are returned
        var all = await handler.Handle(
            new ListMyMaintenanceRequestsQuery { Paging = new PageRequest() },
            CancellationToken.None);

        // Act - status filter narrows the result further
        var completedOnly = await handler.Handle(
            new ListMyMaintenanceRequestsQuery { StatusId = 5, Paging = new PageRequest() },
            CancellationToken.None);

        // Assert
        Assert.Equal(2, all.TotalItems);
        Assert.DoesNotContain(all.Items, r => r.Title == "Foreign request");

        var completed = Assert.Single(completedOnly.Items);
        Assert.Equal("My completed request", completed.Title);
    }

    private static MaintenanceRequestEntity NewRequest(
        int createdByUserId, int buildingId, int statusId, string title)
    {
        return new MaintenanceRequestEntity
        {
            TenantId = 1,
            BuildingId = buildingId,
            CreatedByUserId = createdByUserId,
            PriorityId = 1,
            StatusId = statusId,
            Title = title,
            Description = "Test description for the request."
        };
    }
}

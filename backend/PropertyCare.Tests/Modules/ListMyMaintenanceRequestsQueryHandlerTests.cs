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

    /// <summary>
    /// The date range is a pair of calendar days, so asking for a single day has to return
    /// everything created on it - including the very last minute - and nothing from the days
    /// on either side.
    /// </summary>
    [Fact]
    public async Task Handle_DateRangeOfASingleDay_CoversThatWholeDayAndNothingElse()
    {
        await using var ctx = TestDbContextFactory.Create();
        var me = TestData.AddUser(ctx, roleId: 3, email: "me@test.ba");
        var building = TestData.AddBuilding(ctx);

        var theDay = new DateOnly(2026, 9, 6);

        ctx.MaintenanceRequests.AddRange(
            NewRequest(me.Id, building.Id, 1, "Day before", new DateTime(2026, 9, 5, 23, 59, 0, DateTimeKind.Utc)),
            NewRequest(me.Id, building.Id, 1, "First minute", new DateTime(2026, 9, 6, 0, 0, 0, DateTimeKind.Utc)),
            NewRequest(me.Id, building.Id, 1, "Last minute", new DateTime(2026, 9, 6, 23, 59, 0, DateTimeKind.Utc)),
            NewRequest(me.Id, building.Id, 1, "Day after", new DateTime(2026, 9, 7, 0, 0, 0, DateTimeKind.Utc)));
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new ListMyMaintenanceRequestsQueryHandler(
            ctx, new FakeCurrentUser { UserId = me.Id });

        var result = await handler.Handle(
            new ListMyMaintenanceRequestsQuery
            {
                DateFrom = theDay,
                DateTo = theDay,
                Paging = new PageRequest()
            },
            CancellationToken.None);

        Assert.Equal(2, result.TotalItems);
        Assert.Contains(result.Items, r => r.Title == "First minute");
        Assert.Contains(result.Items, r => r.Title == "Last minute");
        Assert.DoesNotContain(result.Items, r => r.Title == "Day before");
        Assert.DoesNotContain(result.Items, r => r.Title == "Day after");
    }

    private static MaintenanceRequestEntity NewRequest(
        int createdByUserId, int buildingId, int statusId, string title, DateTime? createdAtUtc = null)
    {
        return new MaintenanceRequestEntity
        {
            TenantId = 1,
            BuildingId = buildingId,
            CreatedByUserId = createdByUserId,
            PriorityId = 1,
            StatusId = statusId,
            Title = title,
            Description = "Test description for the request.",
            CreatedAtUtc = createdAtUtc ?? default
        };
    }
}

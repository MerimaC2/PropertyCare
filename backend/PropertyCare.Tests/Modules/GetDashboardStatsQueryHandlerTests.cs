using System.Linq;
using PropertyCare.Application.Modules.Dashboard.Queries.GetStats;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class GetDashboardStatsQueryHandlerTests
{
    [Fact]
    public async Task GetStats_AggregatesTotalsByStatusAndPriority()
    {
        await using var ctx = TestDbContextFactory.Create();
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);

        AddRequest(ctx, building.Id, reporter.Id, statusId: 1, priorityId: 2);
        AddRequest(ctx, building.Id, reporter.Id, statusId: 1, priorityId: 3);
        AddRequest(ctx, building.Id, reporter.Id, statusId: 2, priorityId: 3);
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new GetDashboardStatsQueryHandler(
            ctx, new FakeCurrentUser { UserId = reporter.Id, TenantId = 1 });

        var stats = await handler.Handle(new GetDashboardStatsQuery(), CancellationToken.None);

        Assert.Equal(3, stats.TotalRequests);
        Assert.Equal(2, stats.ByStatus.Count);
        Assert.Equal(3, stats.ByStatus.Sum(s => s.Count));
        Assert.Equal(2, stats.ByPriority.Count);
        Assert.Equal(3, stats.ByPriority.Sum(p => p.Count));
    }

    private static void AddRequest(
        DatabaseContext ctx, int buildingId, int createdByUserId, int statusId, int priorityId)
    {
        ctx.MaintenanceRequests.Add(new MaintenanceRequestEntity
        {
            TenantId = 1,
            BuildingId = buildingId,
            CreatedByUserId = createdByUserId,
            PriorityId = priorityId,
            StatusId = statusId,
            Title = "Test request",
            Description = "Test description"
        });
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Interventions.Commands.AddWorkLog;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class InterventionHandlersTests
{
    [Fact]
    public async Task AddWorkLog_OwnWorkOrder_CreatesLog()
    {
        await using var ctx = TestDbContextFactory.Create();
        var technician = TestData.AddUser(ctx, roleId: 2, email: "tech@test.ba");
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        var workOrder = AddWorkOrder(ctx, building.Id, reporter.Id, technician.Id);

        var handler = new AddWorkLogCommandHandler(
            ctx, new FakeCurrentUser { UserId = technician.Id, TenantId = 1 });

        var id = await handler.Handle(
            new AddWorkLogCommand { WorkOrderId = workOrder.Id, Note = "Replaced part", MinutesSpent = 45 },
            CancellationToken.None);

        var saved = await ctx.WorkLogs.SingleAsync(l => l.Id == id);
        Assert.Equal(workOrder.Id, saved.WorkOrderId);
        Assert.Equal(45, saved.MinutesSpent);
        Assert.Equal("Replaced part", saved.Note);
    }

    [Fact]
    public async Task AddWorkLog_WorkOrderOfAnotherTechnician_ThrowsNotFound()
    {
        await using var ctx = TestDbContextFactory.Create();
        var owner = TestData.AddUser(ctx, roleId: 2, email: "owner@test.ba");
        var other = TestData.AddUser(ctx, roleId: 2, email: "other@test.ba");
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        var workOrder = AddWorkOrder(ctx, building.Id, reporter.Id, owner.Id);

        var handler = new AddWorkLogCommandHandler(
            ctx, new FakeCurrentUser { UserId = other.Id, TenantId = 1 });

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(
                new AddWorkLogCommand { WorkOrderId = workOrder.Id, Note = "x", MinutesSpent = 10 },
                CancellationToken.None));
    }

    private static WorkOrderEntity AddWorkOrder(
        DatabaseContext ctx, int buildingId, int reporterId, int technicianId)
    {
        var request = new MaintenanceRequestEntity
        {
            TenantId = 1,
            BuildingId = buildingId,
            CreatedByUserId = reporterId,
            PriorityId = 2,
            StatusId = 2, // ASSIGNED
            Title = "Broken AC",
            Description = "AC not working"
        };
        ctx.MaintenanceRequests.Add(request);
        ctx.SaveChanges();

        var workOrder = new WorkOrderEntity
        {
            TenantId = 1,
            RequestId = request.Id,
            AssignedToUserId = technicianId,
            StatusId = 1 // ASSIGNED (work-order status static seed)
        };
        ctx.WorkOrders.Add(workOrder);
        ctx.SaveChanges();
        return workOrder;
    }
}

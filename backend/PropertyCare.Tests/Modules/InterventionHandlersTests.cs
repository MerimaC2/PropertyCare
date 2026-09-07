using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Interventions.Commands.AddWorkLog;
using PropertyCare.Application.Modules.Interventions.Queries.ListMine;
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

    [Theory]
    [InlineData(4)] // COMPLETED
    [InlineData(5)] // CANCELLED
    public async Task AddWorkLog_ClosedWorkOrder_ThrowsConflictException(int terminalStatusId)
    {
        await using var ctx = TestDbContextFactory.Create();
        var technician = TestData.AddUser(ctx, roleId: 2, email: "tech@test.ba");
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        var workOrder = AddWorkOrder(
            ctx, building.Id, reporter.Id, technician.Id, statusId: terminalStatusId);

        var handler = new AddWorkLogCommandHandler(
            ctx, new FakeCurrentUser { UserId = technician.Id, TenantId = 1 });

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(
                new AddWorkLogCommand
                {
                    WorkOrderId = workOrder.Id,
                    Note = "Extra hours after closing",
                    MinutesSpent = 30
                },
                CancellationToken.None));

        Assert.Empty(await ctx.WorkLogs.ToListAsync());
    }

    [Fact]
    public async Task ListMine_ProjectsWhetherTheOrderIsClosed()
    {
        await using var ctx = TestDbContextFactory.Create();
        var technician = TestData.AddUser(ctx, roleId: 2, email: "tech@test.ba");
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        AddWorkOrder(ctx, building.Id, reporter.Id, technician.Id, statusId: 1); // ASSIGNED
        AddWorkOrder(ctx, building.Id, reporter.Id, technician.Id, statusId: 4); // COMPLETED

        var handler = new ListMyInterventionsQueryHandler(
            ctx, new FakeCurrentUser { UserId = technician.Id, TenantId = 1 });

        var items = await handler.Handle(new ListMyInterventionsQuery(), CancellationToken.None);

        Assert.Equal(2, items.Count);
        Assert.Contains(items, i => !i.StatusIsTerminal);
        Assert.Contains(items, i => i.StatusIsTerminal);
    }

    private static WorkOrderEntity AddWorkOrder(
        DatabaseContext ctx, int buildingId, int reporterId, int technicianId, int statusId = 1)
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
            StatusId = statusId // work-order status from the static seed
        };
        ctx.WorkOrders.Add(workOrder);
        ctx.SaveChanges();
        return workOrder;
    }
}

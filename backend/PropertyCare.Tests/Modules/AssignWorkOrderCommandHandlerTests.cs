using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.WorkOrders.Commands.Assign;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class AssignWorkOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesWorkOrderAndMovesRequestToAssigned()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var technician = TestData.AddUser(ctx, roleId: 2, email: "tech@test.ba");
        var building = TestData.AddBuilding(ctx);

        var request = new MaintenanceRequestEntity
        {
            TenantId = 1,
            BuildingId = building.Id,
            CreatedByUserId = reporter.Id,
            PriorityId = 3,
            StatusId = 1, // NEW
            Title = "Elevator failure",
            Description = "Elevator is stuck on the 3rd floor."
        };
        ctx.MaintenanceRequests.Add(request);
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new AssignWorkOrderCommandHandler(ctx, new FakeCurrentUser { UserId = admin.Id });
        var command = new AssignWorkOrderCommand
        {
            RequestId = request.Id,
            AssignedToUserId = technician.Id,
            Note = "Check the control panel."
        };

        // Act
        var workOrderId = await handler.Handle(command, CancellationToken.None);

        // Assert
        var workOrder = await ctx.WorkOrders.SingleAsync(w => w.Id == workOrderId);
        Assert.Equal(technician.Id, workOrder.AssignedToUserId);
        Assert.Equal(1, workOrder.StatusId); // ASSIGNED

        var updatedRequest = await ctx.MaintenanceRequests.SingleAsync(r => r.Id == request.Id);
        Assert.Equal(2, updatedRequest.StatusId); // request moved to ASSIGNED

        var history = await ctx.RequestStatusHistories.SingleAsync(h => h.RequestId == request.Id);
        Assert.Equal(admin.Id, history.ChangedByUserId);
    }

    [Fact]
    public async Task Handle_RequestAlreadyAssigned_ThrowsConflictException()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var admin = TestData.AddUser(ctx, roleId: 1, email: "admin@test.ba");
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var technician = TestData.AddUser(ctx, roleId: 2, email: "tech@test.ba");
        var building = TestData.AddBuilding(ctx);

        var request = new MaintenanceRequestEntity
        {
            TenantId = 1,
            BuildingId = building.Id,
            CreatedByUserId = reporter.Id,
            PriorityId = 3,
            StatusId = 2, // ASSIGNED
            Title = "Elevator failure",
            Description = "Elevator is stuck on the 3rd floor."
        };
        ctx.MaintenanceRequests.Add(request);
        ctx.WorkOrders.Add(new WorkOrderEntity
        {
            TenantId = 1,
            Request = request,
            AssignedToUserId = technician.Id,
            StatusId = 1 // ASSIGNED (active)
        });
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new AssignWorkOrderCommandHandler(ctx, new FakeCurrentUser { UserId = admin.Id });
        var command = new AssignWorkOrderCommand
        {
            RequestId = request.Id,
            AssignedToUserId = technician.Id
        };

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}

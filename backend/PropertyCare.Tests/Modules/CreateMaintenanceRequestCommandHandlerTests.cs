using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Modules.MaintenanceRequests.Commands.Create;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class CreateMaintenanceRequestCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesRequestWithInitialStatusHistory()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        var currentUser = new FakeCurrentUser { UserId = reporter.Id };
        var handler = new CreateMaintenanceRequestCommandHandler(ctx, currentUser);

        var command = new CreateMaintenanceRequestCommand
        {
            Title = "Broken radiator",
            Description = "The radiator in the office is leaking water.",
            BuildingId = building.Id,
            PriorityId = 2
        };

        // Act
        var requestId = await handler.Handle(command, CancellationToken.None);

        // Assert
        var request = await ctx.MaintenanceRequests
            .Include(r => r.StatusHistory)
            .SingleAsync(r => r.Id == requestId);

        Assert.Equal(reporter.Id, request.CreatedByUserId);
        Assert.Equal("Broken radiator", request.Title);
        Assert.Equal(1, request.StatusId); // NEW
        var history = Assert.Single(request.StatusHistory);
        Assert.Null(history.FromStatusId);
        Assert.Equal(1, history.ToStatusId);
    }

    [Fact]
    public async Task Handle_UnknownBuilding_ThrowsValidationException()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var handler = new CreateMaintenanceRequestCommandHandler(
            ctx, new FakeCurrentUser { UserId = reporter.Id });

        var command = new CreateMaintenanceRequestCommand
        {
            Title = "Broken radiator",
            Description = "The radiator in the office is leaking water.",
            BuildingId = 9999,
            PriorityId = 2
        };

        // Act + Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}

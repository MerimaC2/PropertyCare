using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.MaintenanceRequests.Commands.UploadImage;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class UploadRequestImageCommandHandlerTests
{
    [Fact]
    public async Task Handle_OwnedRequest_StoresImageAndReturnsUrl()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        var request = AddRequest(ctx, building.Id, reporter.Id);

        var storage = new FakeFileStorageService();
        var handler = new UploadRequestImageCommandHandler(
            ctx, new FakeCurrentUser { UserId = reporter.Id }, storage);

        await using var content = new MemoryStream([1, 2, 3]);
        var command = new UploadRequestImageCommand
        {
            RequestId = request.Id,
            FileName = "photo.jpg",
            ContentType = "image/jpeg",
            SizeBytes = 3,
            Content = content
        };

        // Act
        var dto = await handler.Handle(command, CancellationToken.None);

        // Assert
        var saved = await ctx.RequestImages.SingleAsync();
        Assert.Equal(request.Id, saved.RequestId);
        Assert.Equal("photo.jpg", saved.FileName);
        Assert.Equal("image/jpeg", saved.ContentType);
        Assert.Equal(3, saved.SizeBytes);
        Assert.Equal("/" + saved.RelativePath, dto.Url);
        Assert.Equal($"uploads/{request.Id}", storage.LastSubfolder);
    }

    [Fact]
    public async Task Handle_RequestNotOwnedByCurrentUser_ThrowsNotFound()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var owner = TestData.AddUser(ctx, roleId: 3, email: "owner@test.ba");
        var other = TestData.AddUser(ctx, roleId: 3, email: "other@test.ba");
        var building = TestData.AddBuilding(ctx);
        var request = AddRequest(ctx, building.Id, owner.Id);

        var handler = new UploadRequestImageCommandHandler(
            ctx, new FakeCurrentUser { UserId = other.Id }, new FakeFileStorageService());

        await using var content = new MemoryStream([1]);
        var command = new UploadRequestImageCommand
        {
            RequestId = request.Id,
            FileName = "x.jpg",
            ContentType = "image/jpeg",
            SizeBytes = 1,
            Content = content
        };

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    private static MaintenanceRequestEntity AddRequest(
        DatabaseContext ctx, int buildingId, int createdByUserId)
    {
        var request = new MaintenanceRequestEntity
        {
            TenantId = 1,
            BuildingId = buildingId,
            CreatedByUserId = createdByUserId,
            PriorityId = 2,
            StatusId = 1,
            Title = "Test request",
            Description = "Test description"
        };
        ctx.MaintenanceRequests.Add(request);
        ctx.SaveChanges();
        return request;
    }
}

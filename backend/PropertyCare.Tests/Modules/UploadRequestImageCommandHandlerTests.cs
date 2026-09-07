using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.MaintenanceRequests.Commands.UploadImage;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class UploadRequestImageCommandHandlerTests
{
    private static readonly byte[] JpegBytes = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46];
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] HtmlBytes = "<html><script>alert(1)</script>"u8.ToArray();

    [Fact]
    public async Task Handle_OwnedRequest_StoresImageAndReturnsAuthorizedContentUrl()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        var request = AddRequest(ctx, building.Id, reporter.Id);

        var storage = new FakeFileStorageService();
        var handler = new UploadRequestImageCommandHandler(
            ctx, new FakeCurrentUser { UserId = reporter.Id }, storage);

        await using var content = new MemoryStream(JpegBytes);
        var command = BuildCommand(request.Id, "photo.jpg", "image/jpeg", content);

        // Act
        var dto = await handler.Handle(command, CancellationToken.None);

        // Assert
        var saved = await ctx.RequestImages.SingleAsync();
        Assert.Equal(request.Id, saved.RequestId);
        Assert.Equal("photo.jpg", saved.FileName);
        Assert.Equal("image/jpeg", saved.ContentType);
        Assert.Equal($"uploads/{request.Id}", storage.LastSubfolder);
        Assert.Equal($"/api/maintenance-requests/{request.Id}/images/{saved.Id}/content", dto.Url);
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

        await using var content = new MemoryStream(JpegBytes);
        var command = BuildCommand(request.Id, "x.jpg", "image/jpeg", content);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ContentDoesNotMatchDeclaredType_ThrowsValidationAndStoresNothing()
    {
        // Arrange: an HTML payload announced as a JPEG, which is what a forged header looks like.
        await using var ctx = TestDbContextFactory.Create();
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        var request = AddRequest(ctx, building.Id, reporter.Id);

        var storage = new FakeFileStorageService();
        var handler = new UploadRequestImageCommandHandler(
            ctx, new FakeCurrentUser { UserId = reporter.Id }, storage);

        await using var content = new MemoryStream(HtmlBytes);
        var command = BuildCommand(request.Id, "payload.jpg", "image/jpeg", content);

        // Act + Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Empty(storage.SavedPaths);
        Assert.Empty(await ctx.RequestImages.ToListAsync());
    }

    [Fact]
    public async Task Handle_ClientFileNameHasForeignExtension_StoresServerChosenExtension()
    {
        // Arrange: real PNG content, but the client claims an .html file name.
        await using var ctx = TestDbContextFactory.Create();
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        var request = AddRequest(ctx, building.Id, reporter.Id);

        var storage = new FakeFileStorageService();
        var handler = new UploadRequestImageCommandHandler(
            ctx, new FakeCurrentUser { UserId = reporter.Id }, storage);

        await using var content = new MemoryStream(PngBytes);
        var command = BuildCommand(request.Id, "payload.html", "image/png", content);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(".png", storage.LastExtension);
        Assert.EndsWith(".png", storage.SavedPaths.Single(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Handle_DatabaseWriteFails_DeletesTheStoredFile()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.CreateFailing();
        var reporter = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var building = TestData.AddBuilding(ctx);
        var request = AddRequest(ctx, building.Id, reporter.Id);

        var storage = new FakeFileStorageService();
        var handler = new UploadRequestImageCommandHandler(
            ctx, new FakeCurrentUser { UserId = reporter.Id }, storage);

        await using var content = new MemoryStream(JpegBytes);
        var command = BuildCommand(request.Id, "photo.jpg", "image/jpeg", content);

        ctx.FailOnSave = true;

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(storage.SavedPaths.Single(), storage.DeletedPaths.Single());
    }

    private static UploadRequestImageCommand BuildCommand(
        int requestId, string fileName, string contentType, Stream content)
        => new()
        {
            RequestId = requestId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = content.Length,
            Content = content
        };

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

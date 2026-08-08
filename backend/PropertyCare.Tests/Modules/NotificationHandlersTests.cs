using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Modules.Notifications.Commands.MarkRead;
using PropertyCare.Application.Modules.Notifications.Queries.ListMy;
using PropertyCare.Domain.Entities.System;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class NotificationHandlersTests
{
    [Fact]
    public async Task ListMy_FiltersByReadStateAndType_ForCurrentUserOnly()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var user = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var other = TestData.AddUser(ctx, roleId: 3, email: "other@test.ba");
        AddNotification(ctx, user.Id, NotificationType.RequestSubmitted, isRead: false);
        AddNotification(ctx, user.Id, NotificationType.Assigned, isRead: true);
        AddNotification(ctx, user.Id, NotificationType.Assigned, isRead: false);
        AddNotification(ctx, other.Id, NotificationType.Assigned, isRead: false); // different user, must be excluded
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new ListMyNotificationsQueryHandler(ctx, new FakeCurrentUser { UserId = user.Id });

        // Act + Assert: unread only
        var unread = await handler.Handle(new ListMyNotificationsQuery { IsRead = false }, CancellationToken.None);
        Assert.Equal(2, unread.TotalItems);

        // type Assigned only
        var assigned = await handler.Handle(
            new ListMyNotificationsQuery { Type = NotificationType.Assigned }, CancellationToken.None);
        Assert.Equal(2, assigned.TotalItems);

        // unread AND Assigned
        var both = await handler.Handle(
            new ListMyNotificationsQuery { IsRead = false, Type = NotificationType.Assigned }, CancellationToken.None);
        Assert.Equal(1, both.TotalItems);
    }

    [Fact]
    public async Task MarkRead_MarksNotificationAndReturnsRemainingUnreadCount()
    {
        // Arrange
        await using var ctx = TestDbContextFactory.Create();
        var user = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var first = AddNotification(ctx, user.Id, NotificationType.Assigned, isRead: false);
        AddNotification(ctx, user.Id, NotificationType.RequestSubmitted, isRead: false);
        await ctx.SaveChangesAsync(CancellationToken.None);

        var handler = new MarkNotificationReadCommandHandler(ctx, new FakeCurrentUser { UserId = user.Id });

        // Act
        var remainingUnread = await handler.Handle(
            new MarkNotificationReadCommand { Id = first.Id }, CancellationToken.None);

        // Assert
        Assert.Equal(1, remainingUnread);
        var reloaded = await ctx.Notifications.FindAsync(first.Id);
        Assert.True(reloaded!.IsRead);
    }

    private static NotificationEntity AddNotification(
        DatabaseContext ctx, int userId, NotificationType type, bool isRead)
    {
        var notification = new NotificationEntity
        {
            TenantId = 1,
            UserId = userId,
            Type = type,
            Title = "Test",
            Message = "Test message",
            IsRead = isRead
        };
        ctx.Notifications.Add(notification);
        return notification;
    }
}

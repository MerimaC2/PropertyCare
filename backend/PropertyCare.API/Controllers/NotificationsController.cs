using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Common;
using PropertyCare.Application.Modules.Notifications;
using PropertyCare.Application.Modules.Notifications.Commands.MarkAllRead;
using PropertyCare.Application.Modules.Notifications.Commands.MarkRead;
using PropertyCare.Application.Modules.Notifications.Queries.ListMy;
using PropertyCare.Application.Modules.Notifications.Queries.UnreadCount;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController(ISender sender) : ControllerBase
{
    /// <summary>Paged list of the current user's notifications, filterable by read state and type.</summary>
    [HttpGet("my")]
    public async Task<PageResult<NotificationDto>> ListMy(
        [FromQuery] ListMyNotificationsQuery query,
        CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    /// <summary>Number of unread notifications for the current user.</summary>
    [HttpGet("unread-count")]
    public async Task<int> UnreadCount(CancellationToken ct)
    {
        return await sender.Send(new GetUnreadNotificationCountQuery(), ct);
    }

    /// <summary>Marks a single notification as read; returns the updated unread count.</summary>
    [HttpPost("{id:int}/read")]
    public async Task<int> MarkRead(int id, CancellationToken ct)
    {
        return await sender.Send(new MarkNotificationReadCommand { Id = id }, ct);
    }

    /// <summary>Marks all notifications as read; returns the updated unread count (0).</summary>
    [HttpPost("read-all")]
    public async Task<int> MarkAllRead(CancellationToken ct)
    {
        return await sender.Send(new MarkAllNotificationsReadCommand(), ct);
    }
}

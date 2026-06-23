using PropertyCare.Application.Common;
using PropertyCare.Domain.Entities.System;

namespace PropertyCare.Application.Modules.Notifications.Queries.ListMy;

/// <summary>Paged list of the current user's notifications, filterable by read state and type.</summary>
public sealed class ListMyNotificationsQuery : BasePagedQuery<NotificationDto>
{
    public bool? IsRead { get; set; }
    public NotificationType? Type { get; set; }
}

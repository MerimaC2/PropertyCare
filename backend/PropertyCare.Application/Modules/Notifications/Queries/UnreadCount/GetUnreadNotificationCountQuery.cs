namespace PropertyCare.Application.Modules.Notifications.Queries.UnreadCount;

/// <summary>Number of unread notifications for the current user (drives the toolbar badge).</summary>
public sealed class GetUnreadNotificationCountQuery : IRequest<int> { }

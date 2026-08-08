namespace PropertyCare.Application.Modules.Notifications.Commands.MarkAllRead;

/// <summary>Marks all of the current user's notifications as read; returns the updated unread count (0).</summary>
public sealed class MarkAllNotificationsReadCommand : IRequest<int> { }

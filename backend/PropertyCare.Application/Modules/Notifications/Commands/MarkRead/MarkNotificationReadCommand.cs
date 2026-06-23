namespace PropertyCare.Application.Modules.Notifications.Commands.MarkRead;

/// <summary>Marks one of the current user's notifications as read; returns the updated unread count.</summary>
public sealed class MarkNotificationReadCommand : IRequest<int>
{
    public int Id { get; set; }
}

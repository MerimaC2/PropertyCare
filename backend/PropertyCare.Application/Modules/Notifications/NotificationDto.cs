using PropertyCare.Domain.Entities.System;

namespace PropertyCare.Application.Modules.Notifications;

public sealed class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

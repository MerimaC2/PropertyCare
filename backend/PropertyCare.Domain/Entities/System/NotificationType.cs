namespace PropertyCare.Domain.Entities.System;

/// <summary>Category of an in-app notification, used for grouping and filtering in the notification center.</summary>
public enum NotificationType
{
    General = 0,
    RequestSubmitted = 1,
    Assigned = 2,
    Completed = 3
}

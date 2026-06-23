using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Notifications.Commands.MarkRead;

public sealed class MarkNotificationReadCommandHandler
    : IRequestHandler<MarkNotificationReadCommand, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public MarkNotificationReadCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");

        var notification = await _ctx.Notifications.FirstOrDefaultAsync(
            n => n.Id == request.Id && n.UserId == userId && !n.IsDeleted, ct)
            ?? throw new NotFoundException("Notification not found.");

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _ctx.SaveChangesAsync(ct);
        }

        return await _ctx.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted, ct);
    }
}

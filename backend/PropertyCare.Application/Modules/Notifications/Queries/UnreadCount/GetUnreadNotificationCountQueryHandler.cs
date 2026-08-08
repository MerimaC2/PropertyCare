using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Notifications.Queries.UnreadCount;

public sealed class GetUnreadNotificationCountQueryHandler
    : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public GetUnreadNotificationCountQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");

        return await _ctx.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted, ct);
    }
}

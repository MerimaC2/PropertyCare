using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Notifications.Queries.ListMy;

public sealed class ListMyNotificationsQueryHandler
    : IRequestHandler<ListMyNotificationsQuery, PageResult<NotificationDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListMyNotificationsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<PageResult<NotificationDto>> Handle(
        ListMyNotificationsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");

        var query = _ctx.Notifications.AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsDeleted);

        if (request.IsRead.HasValue)
            query = query.Where(n => n.IsRead == request.IsRead.Value);

        if (request.Type.HasValue)
            query = query.Where(n => n.Type == request.Type.Value);

        var projected = query
            .OrderByDescending(n => n.CreatedAtUtc)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAtUtc = n.CreatedAtUtc
            });

        return await PageResult<NotificationDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}

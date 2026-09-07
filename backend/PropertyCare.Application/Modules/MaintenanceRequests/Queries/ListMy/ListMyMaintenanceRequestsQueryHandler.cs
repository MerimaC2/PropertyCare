using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListMy;

public sealed class ListMyMaintenanceRequestsQueryHandler
    : IRequestHandler<ListMyMaintenanceRequestsQuery, PageResult<ListMyMaintenanceRequestsQueryDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListMyMaintenanceRequestsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<PageResult<ListMyMaintenanceRequestsQueryDto>> Handle(
        ListMyMaintenanceRequestsQuery request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");

        var query = _ctx.MaintenanceRequests.AsNoTracking()
            .Where(r => r.CreatedByUserId == userId && !r.IsDeleted);

        // Backend filters (status, priority, building, date range)
        if (request.StatusId.HasValue)
            query = query.Where(r => r.StatusId == request.StatusId);

        if (request.PriorityId.HasValue)
            query = query.Where(r => r.PriorityId == request.PriorityId);

        if (request.BuildingId.HasValue)
            query = query.Where(r => r.BuildingId == request.BuildingId);

        // The picked days are read as UTC calendar days, which is the same scale CreatedAtUtc is
        // stored on. "To" is turned into the start of the next day so the whole day is included.
        if (request.DateFrom.HasValue)
        {
            var from = request.DateFrom.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(r => r.CreatedAtUtc >= from);
        }

        if (request.DateTo.HasValue)
        {
            var toExclusive = request.DateTo.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            query = query.Where(r => r.CreatedAtUtc < toExclusive);
        }

        var projected = query
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new ListMyMaintenanceRequestsQueryDto
            {
                Id = r.Id,
                Title = r.Title,
                BuildingName = r.Building.Name,
                UnitLabel = r.Unit != null ? r.Unit.Label : null,
                AssetName = r.Asset != null ? r.Asset.Name : null,
                PriorityName = r.Priority.Name,
                PriorityAbrv = r.Priority.Abrv,
                StatusName = r.Status.Name,
                StatusAbrv = r.Status.Abrv,
                CreatedAtUtc = r.CreatedAtUtc
            });

        return await PageResult<ListMyMaintenanceRequestsQueryDto>
            .FromQueryableAsync(projected, request.Paging, ct);
    }
}

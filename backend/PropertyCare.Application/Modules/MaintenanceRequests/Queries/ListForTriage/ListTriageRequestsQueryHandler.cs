using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListForTriage;

public sealed class ListTriageRequestsQueryHandler
    : IRequestHandler<ListTriageRequestsQuery, PageResult<ListTriageRequestsQueryDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListTriageRequestsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<PageResult<ListTriageRequestsQueryDto>> Handle(
        ListTriageRequestsQuery request,
        CancellationToken ct)
    {
        // This list had no tenant restriction at all, so triage showed every tenant's requests.
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var query = _ctx.MaintenanceRequests.AsNoTracking()
            .Where(r => r.TenantId == tenantId && !r.IsDeleted);

        // Backend filters
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(r => r.Title.ToLower().Contains(search));
        }

        if (request.StatusId.HasValue)
            query = query.Where(r => r.StatusId == request.StatusId);

        if (request.PriorityId.HasValue)
            query = query.Where(r => r.PriorityId == request.PriorityId);

        if (request.BuildingId.HasValue)
            query = query.Where(r => r.BuildingId == request.BuildingId);

        // Same rule as on the reporter list: UTC calendar days, "to" inclusive.
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

        // Backend column sorting
        query = (request.SortBy?.ToLowerInvariant(), request.SortDesc) switch
        {
            ("title", false) => query.OrderBy(r => r.Title),
            ("title", true) => query.OrderByDescending(r => r.Title),
            ("building", false) => query.OrderBy(r => r.Building.Name),
            ("building", true) => query.OrderByDescending(r => r.Building.Name),
            ("priority", false) => query.OrderBy(r => r.Priority.SlaHours),
            ("priority", true) => query.OrderByDescending(r => r.Priority.SlaHours),
            ("status", false) => query.OrderBy(r => r.Status.Name),
            ("status", true) => query.OrderByDescending(r => r.Status.Name),
            ("createdby", false) => query.OrderBy(r => r.CreatedByUser.FirstName).ThenBy(r => r.CreatedByUser.LastName),
            ("createdby", true) => query.OrderByDescending(r => r.CreatedByUser.FirstName).ThenByDescending(r => r.CreatedByUser.LastName),
            (_, false) => query.OrderBy(r => r.CreatedAtUtc),
            _ => query.OrderByDescending(r => r.CreatedAtUtc)
        };

        var projected = query.Select(r => new ListTriageRequestsQueryDto
        {
            Id = r.Id,
            Title = r.Title,
            BuildingName = r.Building.Name,
            UnitLabel = r.Unit != null ? r.Unit.Label : null,
            PriorityName = r.Priority.Name,
            PriorityAbrv = r.Priority.Abrv,
            StatusName = r.Status.Name,
            StatusAbrv = r.Status.Abrv,
            StatusIsTerminal = r.Status.IsTerminal,
            CreatedByName = r.CreatedByUser.FirstName + " " + r.CreatedByUser.LastName,
            AssignedToName = r.WorkOrders
                .Where(w => !w.IsDeleted && !w.Status.IsTerminal)
                .OrderByDescending(w => w.CreatedAtUtc)
                .Select(w => w.AssignedToUser.FirstName + " " + w.AssignedToUser.LastName)
                .FirstOrDefault(),
            CreatedAtUtc = r.CreatedAtUtc
        });

        return await PageResult<ListTriageRequestsQueryDto>
            .FromQueryableAsync(projected, request.Paging, ct);
    }
}

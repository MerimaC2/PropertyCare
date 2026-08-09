using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Dashboard.Queries.GetStats;

public sealed class GetDashboardStatsQueryHandler
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public GetDashboardStatsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var requests = _ctx.MaintenanceRequests.AsNoTracking()
            .Where(r => r.TenantId == tenantId && !r.IsDeleted);

        var total = await requests.CountAsync(ct);

        var byStatus = (await requests
                .GroupBy(r => r.Status.Name)
                .Select(g => new CountByLabelDto { Label = g.Key, Count = g.Count() })
                .ToListAsync(ct))
            .OrderBy(x => x.Label)
            .ToList();

        var byPriority = (await requests
                .GroupBy(r => r.Priority.Name)
                .Select(g => new CountByLabelDto { Label = g.Key, Count = g.Count() })
                .ToListAsync(ct))
            .OrderBy(x => x.Label)
            .ToList();

        return new DashboardStatsDto
        {
            TotalRequests = total,
            ByStatus = byStatus,
            ByPriority = byPriority
        };
    }
}

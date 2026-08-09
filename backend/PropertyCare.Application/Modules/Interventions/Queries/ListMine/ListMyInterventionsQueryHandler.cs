using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Interventions.Queries.ListMine;

public sealed class ListMyInterventionsQueryHandler
    : IRequestHandler<ListMyInterventionsQuery, IReadOnlyList<InterventionDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListMyInterventionsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<InterventionDto>> Handle(
        ListMyInterventionsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");

        return await _ctx.WorkOrders.AsNoTracking()
            .Where(w => w.AssignedToUserId == userId && !w.IsDeleted)
            .OrderByDescending(w => w.CreatedAtUtc)
            .Select(w => new InterventionDto
            {
                WorkOrderId = w.Id,
                RequestTitle = w.Request.Title,
                BuildingName = w.Request.Building.Name,
                StatusName = w.Status.Name,
                CreatedAtUtc = w.CreatedAtUtc,
                TotalMinutes = w.WorkLogs.Where(l => !l.IsDeleted).Sum(l => l.MinutesSpent),
                Logs = w.WorkLogs
                    .Where(l => !l.IsDeleted)
                    .OrderByDescending(l => l.CreatedAtUtc)
                    .Select(l => new WorkLogDto
                    {
                        Id = l.Id,
                        Note = l.Note,
                        MinutesSpent = l.MinutesSpent,
                        CreatedAtUtc = l.CreatedAtUtc
                    })
                    .ToList()
            })
            .ToListAsync(ct);
    }
}

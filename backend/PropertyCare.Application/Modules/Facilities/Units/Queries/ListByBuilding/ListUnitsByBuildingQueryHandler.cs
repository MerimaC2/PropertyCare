using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Units.Queries.ListByBuilding;

public sealed class ListUnitsByBuildingQueryHandler
    : IRequestHandler<ListUnitsByBuildingQuery, IReadOnlyList<UnitDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListUnitsByBuildingQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<UnitDto>> Handle(
        ListUnitsByBuildingQuery request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        return await _ctx.Units.AsNoTracking()
            .Where(u => u.TenantId == tenantId
                && u.BuildingId == request.BuildingId
                && !u.IsDeleted)
            .OrderBy(u => u.Label)
            .Select(u => new UnitDto
            {
                Id = u.Id,
                BuildingId = u.BuildingId,
                Label = u.Label,
                AssetCount = u.Assets.Count(a => !a.IsDeleted)
            })
            .ToListAsync(ct);
    }
}

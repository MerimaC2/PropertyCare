using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Assets.Queries.ListByUnit;

public sealed class ListAssetsByUnitQueryHandler
    : IRequestHandler<ListAssetsByUnitQuery, IReadOnlyList<AssetDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListAssetsByUnitQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<AssetDto>> Handle(
        ListAssetsByUnitQuery request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        return await _ctx.Assets.AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.UnitId == request.UnitId && !a.IsDeleted)
            .OrderBy(a => a.Name)
            .Select(a => new AssetDto
            {
                Id = a.Id,
                UnitId = a.UnitId,
                Name = a.Name,
                AssetTypeId = a.AssetTypeId,
                AssetTypeName = a.AssetType.Name
            })
            .ToListAsync(ct);
    }
}

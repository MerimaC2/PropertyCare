using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.AssetTypes.Queries.List;

public sealed class ListAssetTypesQueryHandler
    : IRequestHandler<ListAssetTypesQuery, IReadOnlyList<AssetTypeDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListAssetTypesQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<AssetTypeDto>> Handle(
        ListAssetTypesQuery request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        return await _ctx.AssetTypes.AsNoTracking()
            .Where(t => t.TenantId == tenantId && !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Select(t => new AssetTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                DefaultSlaHours = t.DefaultSlaHours,
                AssetCount = t.Assets.Count(a => !a.IsDeleted)
            })
            .ToListAsync(ct);
    }
}

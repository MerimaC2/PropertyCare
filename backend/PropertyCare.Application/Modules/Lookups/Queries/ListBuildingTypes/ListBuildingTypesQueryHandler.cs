using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Lookups.Queries.ListBuildingTypes;

public sealed class ListBuildingTypesQueryHandler
    : IRequestHandler<ListBuildingTypesQuery, IReadOnlyList<LookupItemDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListBuildingTypesQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<LookupItemDto>> Handle(
        ListBuildingTypesQuery request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        return await _ctx.BuildingTypes.AsNoTracking()
            .Where(t => t.TenantId == tenantId && !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Select(t => new LookupItemDto { Id = t.Id, Name = t.Name })
            .ToListAsync(ct);
    }
}

using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Buildings.Queries.Locations;

public sealed class ListBuildingLocationsQueryHandler
    : IRequestHandler<ListBuildingLocationsQuery, IReadOnlyList<BuildingLocationDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListBuildingLocationsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<BuildingLocationDto>> Handle(
        ListBuildingLocationsQuery request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        return await _ctx.Buildings.AsNoTracking()
            .Where(b => b.TenantId == tenantId && !b.IsDeleted
                && b.Latitude != null && b.Longitude != null)
            .OrderBy(b => b.Name)
            .Select(b => new BuildingLocationDto
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Latitude = b.Latitude!.Value,
                Longitude = b.Longitude!.Value
            })
            .ToListAsync(ct);
    }
}

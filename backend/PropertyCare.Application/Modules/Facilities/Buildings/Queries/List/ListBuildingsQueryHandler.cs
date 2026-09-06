using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Buildings.Queries.List;

public sealed class ListBuildingsQueryHandler
    : IRequestHandler<ListBuildingsQuery, PageResult<BuildingDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListBuildingsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<PageResult<BuildingDto>> Handle(ListBuildingsQuery request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var query = _ctx.Buildings.AsNoTracking()
            .Where(b => b.TenantId == tenantId && !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(b => b.Name.Contains(term)
                || (b.Address != null && b.Address.Contains(term)));
        }

        if (request.BuildingTypeId.HasValue)
            query = query.Where(b => b.BuildingTypeId == request.BuildingTypeId);

        if (request.MinUnitCount.HasValue)
            query = query.Where(b => b.Units.Count(u => !u.IsDeleted) >= request.MinUnitCount);

        if (request.MaxUnitCount.HasValue)
            query = query.Where(b => b.Units.Count(u => !u.IsDeleted) <= request.MaxUnitCount);

        if (request.HasLocation.HasValue)
        {
            query = request.HasLocation.Value
                ? query.Where(b => b.Latitude != null && b.Longitude != null)
                : query.Where(b => b.Latitude == null || b.Longitude == null);
        }

        var projected = query
            .OrderBy(b => b.Name)
            .Select(b => new BuildingDto
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                BuildingTypeId = b.BuildingTypeId,
                BuildingTypeName = b.BuildingType.Name,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                UnitCount = b.Units.Count(u => !u.IsDeleted)
            });

        return await PageResult<BuildingDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}

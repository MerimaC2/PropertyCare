using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Lookups.Queries.GetRequestFormLookups;

public sealed class GetRequestFormLookupsQueryHandler
    : IRequestHandler<GetRequestFormLookupsQuery, GetRequestFormLookupsQueryDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public GetRequestFormLookupsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<GetRequestFormLookupsQueryDto> Handle(
        GetRequestFormLookupsQuery request,
        CancellationToken ct)
    {
        // The global tenant filter already narrows every set below; this turns a caller with no
        // tenant into a clear 403 instead of five silently empty lists.
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var buildings = await _ctx.Buildings.AsNoTracking()
            .Where(b => b.TenantId == tenantId && !b.IsDeleted)
            .OrderBy(b => b.Name)
            .Select(b => new LookupItemDto { Id = b.Id, Name = b.Name })
            .ToListAsync(ct);

        var units = await _ctx.Units.AsNoTracking()
            .Where(u => u.TenantId == tenantId && !u.IsDeleted)
            .OrderBy(u => u.Label)
            .Select(u => new UnitLookupDto { Id = u.Id, BuildingId = u.BuildingId, Label = u.Label })
            .ToListAsync(ct);

        var assets = await _ctx.Assets.AsNoTracking()
            .Where(a => a.TenantId == tenantId && !a.IsDeleted)
            .OrderBy(a => a.Name)
            .Select(a => new AssetLookupDto { Id = a.Id, UnitId = a.UnitId, Name = a.Name })
            .ToListAsync(ct);

        var priorities = await _ctx.RequestPriorities.AsNoTracking()
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .OrderBy(p => p.Id)
            .Select(p => new LookupItemDto { Id = p.Id, Name = p.Name })
            .ToListAsync(ct);

        var statuses = await _ctx.RequestStatuses.AsNoTracking()
            .Where(s => s.TenantId == tenantId && !s.IsDeleted)
            .OrderBy(s => s.Id)
            .Select(s => new LookupItemDto { Id = s.Id, Name = s.Name })
            .ToListAsync(ct);

        return new GetRequestFormLookupsQueryDto
        {
            Buildings = buildings,
            Units = units,
            Assets = assets,
            Priorities = priorities,
            Statuses = statuses
        };
    }
}

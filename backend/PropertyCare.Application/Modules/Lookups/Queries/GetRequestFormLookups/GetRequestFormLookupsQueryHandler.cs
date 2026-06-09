using PropertyCare.Application.Abstractions;

namespace PropertyCare.Application.Modules.Lookups.Queries.GetRequestFormLookups;

public sealed class GetRequestFormLookupsQueryHandler
    : IRequestHandler<GetRequestFormLookupsQuery, GetRequestFormLookupsQueryDto>
{
    private readonly IAppDbContext _ctx;

    public GetRequestFormLookupsQueryHandler(IAppDbContext ctx) => _ctx = ctx;

    public async Task<GetRequestFormLookupsQueryDto> Handle(
        GetRequestFormLookupsQuery request,
        CancellationToken ct)
    {
        var buildings = await _ctx.Buildings.AsNoTracking()
            .Where(b => !b.IsDeleted)
            .OrderBy(b => b.Name)
            .Select(b => new LookupItemDto { Id = b.Id, Name = b.Name })
            .ToListAsync(ct);

        var units = await _ctx.Units.AsNoTracking()
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.Label)
            .Select(u => new UnitLookupDto { Id = u.Id, BuildingId = u.BuildingId, Label = u.Label })
            .ToListAsync(ct);

        var assets = await _ctx.Assets.AsNoTracking()
            .Where(a => !a.IsDeleted)
            .OrderBy(a => a.Name)
            .Select(a => new AssetLookupDto { Id = a.Id, UnitId = a.UnitId, Name = a.Name })
            .ToListAsync(ct);

        var priorities = await _ctx.RequestPriorities.AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Id)
            .Select(p => new LookupItemDto { Id = p.Id, Name = p.Name })
            .ToListAsync(ct);

        var statuses = await _ctx.RequestStatuses.AsNoTracking()
            .Where(s => !s.IsDeleted)
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

using PropertyCare.Application.Abstractions;

namespace PropertyCare.Application.Modules.Lookups.Queries.ListBuildingTypes;

public sealed class ListBuildingTypesQueryHandler
    : IRequestHandler<ListBuildingTypesQuery, IReadOnlyList<LookupItemDto>>
{
    private readonly IAppDbContext _ctx;

    public ListBuildingTypesQueryHandler(IAppDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<LookupItemDto>> Handle(
        ListBuildingTypesQuery request, CancellationToken ct)
    {
        return await _ctx.BuildingTypes.AsNoTracking()
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Select(t => new LookupItemDto { Id = t.Id, Name = t.Name })
            .ToListAsync(ct);
    }
}

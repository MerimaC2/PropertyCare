namespace PropertyCare.Application.Modules.Lookups.Queries.GetRequestFormLookups;

public sealed class GetRequestFormLookupsQueryDto
{
    public List<LookupItemDto> Buildings { get; init; } = [];
    public List<UnitLookupDto> Units { get; init; } = [];
    public List<AssetLookupDto> Assets { get; init; } = [];
    public List<LookupItemDto> Priorities { get; init; } = [];
    public List<LookupItemDto> Statuses { get; init; } = [];
}

public sealed class UnitLookupDto
{
    public int Id { get; init; }
    public int BuildingId { get; init; }
    public string Label { get; init; } = null!;
}

public sealed class AssetLookupDto
{
    public int Id { get; init; }
    public int UnitId { get; init; }
    public string Name { get; init; } = null!;
}

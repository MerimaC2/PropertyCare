namespace PropertyCare.Application.Modules.Facilities.Assets.Queries.ListByUnit;

/// <summary>Assets belonging to a unit (admin master-detail view).</summary>
public sealed class ListAssetsByUnitQuery : IRequest<IReadOnlyList<AssetDto>>
{
    public int UnitId { get; set; }
}

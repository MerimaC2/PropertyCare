namespace PropertyCare.Application.Modules.Facilities.Units.Queries.ListByBuilding;

/// <summary>Units belonging to a building (admin master-detail view).</summary>
public sealed class ListUnitsByBuildingQuery : IRequest<IReadOnlyList<UnitDto>>
{
    public int BuildingId { get; set; }
}

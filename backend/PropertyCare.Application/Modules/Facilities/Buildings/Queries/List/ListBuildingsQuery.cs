using PropertyCare.Application.Common;

namespace PropertyCare.Application.Modules.Facilities.Buildings.Queries.List;

/// <summary>
/// Paged list of buildings for admin management, with 5 filter parameters:
/// search term, building type, unit count range and whether the building is placed on the map.
/// </summary>
public sealed class ListBuildingsQuery : BasePagedQuery<BuildingDto>
{
    /// <summary>Matches the name or the address.</summary>
    public string? Search { get; set; }

    public int? BuildingTypeId { get; set; }

    public int? MinUnitCount { get; set; }
    public int? MaxUnitCount { get; set; }

    /// <summary>True: only buildings with coordinates. False: only buildings still missing them.</summary>
    public bool? HasLocation { get; set; }
}

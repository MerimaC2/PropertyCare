using PropertyCare.Application.Common;

namespace PropertyCare.Application.Modules.Facilities.Buildings.Queries.List;

/// <summary>Paged list of buildings for admin management, optionally filtered by a search term.</summary>
public sealed class ListBuildingsQuery : BasePagedQuery<BuildingDto>
{
    public string? Search { get; set; }
}

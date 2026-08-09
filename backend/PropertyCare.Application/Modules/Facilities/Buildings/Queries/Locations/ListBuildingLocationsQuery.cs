namespace PropertyCare.Application.Modules.Facilities.Buildings.Queries.Locations;

/// <summary>All buildings that have coordinates, for the interactive map.</summary>
public sealed class ListBuildingLocationsQuery : IRequest<IReadOnlyList<BuildingLocationDto>> { }

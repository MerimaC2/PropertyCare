namespace PropertyCare.Application.Modules.Facilities.Buildings.Queries.Locations;

/// <summary>A building that has map coordinates, for plotting on the interactive map.</summary>
public sealed class BuildingLocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

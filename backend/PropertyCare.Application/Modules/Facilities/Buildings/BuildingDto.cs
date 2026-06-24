namespace PropertyCare.Application.Modules.Facilities.Buildings;

public sealed class BuildingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public int BuildingTypeId { get; set; }
    public string BuildingTypeName { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int UnitCount { get; set; }
}

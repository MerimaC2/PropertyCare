namespace PropertyCare.Application.Modules.Facilities.Units;

public sealed class UnitDto
{
    public int Id { get; set; }
    public int BuildingId { get; set; }
    public string Label { get; set; } = null!;
    public int AssetCount { get; set; }
}

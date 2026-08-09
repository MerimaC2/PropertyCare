namespace PropertyCare.Application.Modules.Facilities.Assets;

public sealed class AssetDto
{
    public int Id { get; set; }
    public int UnitId { get; set; }
    public string Name { get; set; } = null!;
    public int AssetTypeId { get; set; }
    public string AssetTypeName { get; set; } = null!;
}

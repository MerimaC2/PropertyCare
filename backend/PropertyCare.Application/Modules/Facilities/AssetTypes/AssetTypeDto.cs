namespace PropertyCare.Application.Modules.Facilities.AssetTypes;

public sealed class AssetTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? DefaultSlaHours { get; set; }
    public int AssetCount { get; set; }
}

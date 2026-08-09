namespace PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Update;

public sealed class UpdateAssetTypeCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? DefaultSlaHours { get; set; }
}

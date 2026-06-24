namespace PropertyCare.Application.Modules.Facilities.Assets.Commands.Update;

public sealed class UpdateAssetCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int AssetTypeId { get; set; }
}

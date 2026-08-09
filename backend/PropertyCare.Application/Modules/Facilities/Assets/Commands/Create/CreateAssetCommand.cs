namespace PropertyCare.Application.Modules.Facilities.Assets.Commands.Create;

public sealed class CreateAssetCommand : IRequest<int>
{
    public int UnitId { get; set; }
    public string Name { get; set; } = null!;
    public int AssetTypeId { get; set; }
}

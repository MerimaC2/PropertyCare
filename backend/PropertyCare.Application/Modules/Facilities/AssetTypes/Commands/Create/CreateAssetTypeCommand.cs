namespace PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Create;

public sealed class CreateAssetTypeCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
    public int? DefaultSlaHours { get; set; }
}

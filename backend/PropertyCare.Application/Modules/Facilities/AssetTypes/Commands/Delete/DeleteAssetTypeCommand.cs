namespace PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Delete;

public sealed class DeleteAssetTypeCommand : IRequest
{
    public int Id { get; set; }
}

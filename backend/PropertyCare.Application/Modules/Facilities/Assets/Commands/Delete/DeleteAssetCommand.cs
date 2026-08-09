namespace PropertyCare.Application.Modules.Facilities.Assets.Commands.Delete;

public sealed class DeleteAssetCommand : IRequest
{
    public int Id { get; set; }
}

namespace PropertyCare.Application.Modules.Facilities.Buildings.Commands.Delete;

public sealed class DeleteBuildingCommand : IRequest
{
    public int Id { get; set; }
}

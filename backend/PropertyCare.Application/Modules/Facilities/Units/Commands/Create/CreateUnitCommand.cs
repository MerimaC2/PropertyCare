namespace PropertyCare.Application.Modules.Facilities.Units.Commands.Create;

public sealed class CreateUnitCommand : IRequest<int>
{
    public int BuildingId { get; set; }
    public string Label { get; set; } = null!;
}

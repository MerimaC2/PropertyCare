namespace PropertyCare.Application.Modules.Facilities.Units.Commands.Update;

public sealed class UpdateUnitCommand : IRequest
{
    public int Id { get; set; }
    public string Label { get; set; } = null!;
}

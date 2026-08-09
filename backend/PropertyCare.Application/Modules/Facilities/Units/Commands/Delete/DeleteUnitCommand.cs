namespace PropertyCare.Application.Modules.Facilities.Units.Commands.Delete;

public sealed class DeleteUnitCommand : IRequest
{
    public int Id { get; set; }
}

namespace PropertyCare.Application.Modules.Facilities.Buildings.Commands.Create;

public sealed class CreateBuildingCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public int BuildingTypeId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

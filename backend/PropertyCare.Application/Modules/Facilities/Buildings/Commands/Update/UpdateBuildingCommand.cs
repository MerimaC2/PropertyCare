namespace PropertyCare.Application.Modules.Facilities.Buildings.Commands.Update;

public sealed class UpdateBuildingCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public int BuildingTypeId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

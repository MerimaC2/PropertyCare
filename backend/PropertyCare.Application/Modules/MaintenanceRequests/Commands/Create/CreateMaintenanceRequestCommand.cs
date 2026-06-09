namespace PropertyCare.Application.Modules.MaintenanceRequests.Commands.Create;

public sealed class CreateMaintenanceRequestCommand : IRequest<int>
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int BuildingId { get; set; }
    public int? UnitId { get; set; }
    public int? AssetId { get; set; }
    public int PriorityId { get; set; }
}

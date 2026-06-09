namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListMy;

public sealed class ListMyMaintenanceRequestsQueryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string BuildingName { get; init; } = null!;
    public string? UnitLabel { get; init; }
    public string? AssetName { get; init; }
    public string PriorityName { get; init; } = null!;
    public string PriorityAbrv { get; init; } = null!;
    public string StatusName { get; init; } = null!;
    public string StatusAbrv { get; init; } = null!;
    public DateTime CreatedAtUtc { get; init; }
}

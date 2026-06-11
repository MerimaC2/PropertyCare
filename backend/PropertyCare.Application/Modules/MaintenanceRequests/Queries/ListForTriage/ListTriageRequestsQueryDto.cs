namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListForTriage;

public sealed class ListTriageRequestsQueryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string BuildingName { get; init; } = null!;
    public string? UnitLabel { get; init; }
    public string PriorityName { get; init; } = null!;
    public string PriorityAbrv { get; init; } = null!;
    public string StatusName { get; init; } = null!;
    public string StatusAbrv { get; init; } = null!;
    public bool StatusIsTerminal { get; init; }
    public string CreatedByName { get; init; } = null!;
    public string? AssignedToName { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

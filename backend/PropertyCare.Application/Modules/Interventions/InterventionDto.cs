namespace PropertyCare.Application.Modules.Interventions;

/// <summary>A technician's work order with its logged work (read-only history view).</summary>
public sealed class InterventionDto
{
    public int WorkOrderId { get; set; }
    public string RequestTitle { get; set; } = null!;
    public string BuildingName { get; set; } = null!;
    public string StatusName { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public int TotalMinutes { get; set; }
    public IReadOnlyList<WorkLogDto> Logs { get; set; } = [];
}

public sealed class WorkLogDto
{
    public int Id { get; set; }
    public string Note { get; set; } = null!;
    public int MinutesSpent { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

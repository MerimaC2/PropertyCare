using PropertyCare.Application.Common;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListForTriage;

/// <summary>
/// Paged, filterable and sortable list of all maintenance requests
/// used by the administrator triage screen.
/// </summary>
public sealed class ListTriageRequestsQuery : BasePagedQuery<ListTriageRequestsQueryDto>
{
    public string? Search { get; set; }
    public int? StatusId { get; set; }
    public int? PriorityId { get; set; }
    public int? BuildingId { get; set; }
    /// <summary>Calendar days, not moments - see <c>ListMyMaintenanceRequestsQuery</c>.</summary>
    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTo { get; set; }

    /// <summary>Column to sort by: title, building, priority, status, createdBy or createdAtUtc (default).</summary>
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; } = true;
}

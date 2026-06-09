using PropertyCare.Application.Common;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListMy;

/// <summary>
/// Paged list of the current user's own maintenance requests
/// with 5 filter parameters: status, priority, building, date from and date to.
/// </summary>
public sealed class ListMyMaintenanceRequestsQuery : BasePagedQuery<ListMyMaintenanceRequestsQueryDto>
{
    public int? StatusId { get; set; }
    public int? PriorityId { get; set; }
    public int? BuildingId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

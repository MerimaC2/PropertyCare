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
    /// <summary>
    /// Calendar days, not moments: the datepicker gives a day, and a day has no time zone. As
    /// <see cref="DateTime"/> the frontend had to send a moment, and local midnight converted to
    /// UTC landed on the previous day.
    /// </summary>
    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTo { get; set; }
}

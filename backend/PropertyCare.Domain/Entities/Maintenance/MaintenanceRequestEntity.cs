using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Maintenance;

/// <summary>A fault report created by a reporter for a building/unit/asset.</summary>
public sealed class MaintenanceRequestEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int BuildingId { get; set; }
    public BuildingEntity Building { get; set; } = null!;

    public int? UnitId { get; set; }
    public UnitEntity? Unit { get; set; }

    public int? AssetId { get; set; }
    public AssetEntity? Asset { get; set; }

    public int CreatedByUserId { get; set; }
    public AppUserEntity CreatedByUser { get; set; } = null!;

    public int PriorityId { get; set; }
    public RequestPriorityEntity Priority { get; set; } = null!;

    public int StatusId { get; set; }
    public RequestStatusEntity Status { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<WorkOrderEntity> WorkOrders { get; set; } = new List<WorkOrderEntity>();
    public ICollection<RequestCommentEntity> Comments { get; set; } = new List<RequestCommentEntity>();
    public ICollection<RequestStatusHistoryEntity> StatusHistory { get; set; } = new List<RequestStatusHistoryEntity>();
    public ICollection<RequestImageEntity> Images { get; set; } = new List<RequestImageEntity>();

    public static class Constraints
    {
        public const int TitleMaxLength = 150;
        public const int DescriptionMaxLength = 2000;
    }
}

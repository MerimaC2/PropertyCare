using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Facilities;

/// <summary>A unit inside a building (office, apartment, room...).</summary>
public sealed class UnitEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int BuildingId { get; set; }
    public BuildingEntity Building { get; set; } = null!;

    public string Label { get; set; } = null!;

    public ICollection<AssetEntity> Assets { get; set; } = new List<AssetEntity>();

    public static class Constraints
    {
        public const int LabelMaxLength = 120;
    }
}

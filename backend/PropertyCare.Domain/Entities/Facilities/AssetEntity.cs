using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Facilities;

/// <summary>A piece of equipment located in a unit (elevator, AC unit, boiler...).</summary>
public sealed class AssetEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int UnitId { get; set; }
    public UnitEntity Unit { get; set; } = null!;

    public int AssetTypeId { get; set; }
    public AssetTypeEntity AssetType { get; set; } = null!;

    public string Name { get; set; } = null!;

    public static class Constraints
    {
        public const int NameMaxLength = 150;
    }
}

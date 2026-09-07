using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Facilities;

public sealed class AssetTypeEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public string Name { get; set; } = null!;

    /// <summary>Default service-level agreement in hours for this asset type.</summary>
    public int? DefaultSlaHours { get; set; }

    public ICollection<AssetEntity> Assets { get; set; } = new List<AssetEntity>();

    public static class Constraints
    {
        public const int NameMaxLength = 120;
    }
}

using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Facilities;

public sealed class BuildingEntity : BaseEntity
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int BuildingTypeId { get; set; }
    public BuildingTypeEntity BuildingType { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Address { get; set; }

    /// <summary>Optional geographic location, used to plot the building on an interactive map.</summary>
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public ICollection<UnitEntity> Units { get; set; } = new List<UnitEntity>();

    public static class Constraints
    {
        public const int NameMaxLength = 120;
        public const int AddressMaxLength = 200;
    }
}

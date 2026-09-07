using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Facilities;

public sealed class BuildingEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int BuildingTypeId { get; set; }
    public BuildingTypeEntity BuildingType { get; set; } = null!;

    public string Name { get; set; } = null!;

    /// <summary>
    /// <see cref="Name"/> reduced to its comparable form. Backs the unique index per tenant, so two
    /// buildings cannot differ only by casing or surrounding whitespace. Always set through
    /// <see cref="NormalizeName"/>.
    /// </summary>
    public string NameNormalized { get; set; } = null!;

    public string? Address { get; set; }

    /// <summary>Optional geographic location, used to plot the building on an interactive map.</summary>
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public ICollection<UnitEntity> Units { get; set; } = new List<UnitEntity>();

    /// <summary>
    /// The single definition of "same name", shared by the write handlers, the availability query
    /// and the migration that backfills the column.
    /// </summary>
    public static string NormalizeName(string name)
        => (name ?? string.Empty).Trim().ToUpperInvariant();

    public static class Constraints
    {
        public const int NameMaxLength = 120;
        public const int AddressMaxLength = 200;
    }
}

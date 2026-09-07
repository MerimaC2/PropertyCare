using PropertyCare.Domain.Common;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Domain.Entities.Facilities;

public sealed class BuildingTypeEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public string Abrv { get; set; } = null!;
    public string Name { get; set; } = null!;

    public ICollection<BuildingEntity> Buildings { get; set; } = new List<BuildingEntity>();

    public static class Constraints
    {
        public const int AbrvMaxLength = 30;
        public const int NameMaxLength = 60;
    }
}

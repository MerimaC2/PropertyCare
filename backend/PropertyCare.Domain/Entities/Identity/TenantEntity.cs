using PropertyCare.Domain.Common;

namespace PropertyCare.Domain.Entities.Identity;

public sealed class TenantEntity : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<AppUserEntity> Users { get; set; } = new List<AppUserEntity>();

    public static class Constraints
    {
        public const int NameMaxLength = 120;
    }
}

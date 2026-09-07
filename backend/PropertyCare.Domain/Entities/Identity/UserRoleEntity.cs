using PropertyCare.Domain.Common;

namespace PropertyCare.Domain.Entities.Identity;

public sealed class UserRoleEntity : BaseEntity, ITenantScoped
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public string Abrv { get; set; } = null!;
    public string Name { get; set; } = null!;

    public ICollection<AppUserEntity> Users { get; set; } = new List<AppUserEntity>();

    public static class Constraints
    {
        public const int AbrvMaxLength = 30;
        public const int NameMaxLength = 60;
    }

    /// <summary>Well-known role names used for authorization checks.</summary>
    public static class Names
    {
        public const string Administrator = "Administrator";
        public const string Technician = "Technician";
        public const string Reporter = "Reporter";
    }
}

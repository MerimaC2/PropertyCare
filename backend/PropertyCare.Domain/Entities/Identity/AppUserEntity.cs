using PropertyCare.Domain.Common;

namespace PropertyCare.Domain.Entities.Identity;

public sealed class AppUserEntity : BaseEntity
{
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; } = null!;

    public int RoleId { get; set; }
    public UserRoleEntity Role { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; }

    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();

    public string FullName => $"{FirstName} {LastName}";

    public static class Constraints
    {
        public const int FirstNameMaxLength = 60;
        public const int LastNameMaxLength = 60;
        public const int EmailMaxLength = 255;
        public const int PasswordHashMaxLength = 500;
    }
}

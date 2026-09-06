using PropertyCare.Domain.Common;

namespace PropertyCare.Domain.Entities.Identity;

public sealed class RefreshTokenEntity : BaseEntity
{
    public int UserId { get; set; }
    public AppUserEntity User { get; set; } = null!;

    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAtUtc { get; set; }

    public static class Constraints
    {
        public const int TokenHashMaxLength = 128;
    }
}

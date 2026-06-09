using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Application.Abstractions;

public sealed class JwtTokenPair
{
    public string AccessToken { get; init; } = null!;
    public DateTime AccessTokenExpiresAtUtc { get; init; }
    public string RefreshTokenRaw { get; init; } = null!;
    public DateTime RefreshTokenExpiresAtUtc { get; init; }
}

public interface IJwtTokenService
{
    /// <summary>Issues an access/refresh token pair. The user's Role navigation must be loaded.</summary>
    JwtTokenPair IssueTokens(AppUserEntity user);

    /// <summary>Hashes a raw refresh token so only the hash is persisted.</summary>
    string HashRefreshToken(string rawToken);
}

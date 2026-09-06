using PropertyCare.Application.Abstractions;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Tests.Common;

/// <summary>Issues predictable tokens so handler tests do not need real signing keys.</summary>
public sealed class FakeJwtTokenService : IJwtTokenService
{
    public JwtTokenPair IssueTokens(AppUserEntity user) => new()
    {
        AccessToken = $"access-{user.Id}",
        AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
        RefreshTokenRaw = $"refresh-{user.Id}",
        RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(14)
    };

    public string HashRefreshToken(string rawToken) => $"hashed:{rawToken}";
}

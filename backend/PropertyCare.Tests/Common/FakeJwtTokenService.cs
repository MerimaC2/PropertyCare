using PropertyCare.Application.Abstractions;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Tests.Common;

/// <summary>Issues predictable tokens so handler tests do not need real signing keys.</summary>
public sealed class FakeJwtTokenService : IJwtTokenService
{
    private int _issueCount;

    public JwtTokenPair IssueTokens(AppUserEntity user)
    {
        // Each call has to produce a different refresh token, otherwise rotation would be
        // indistinguishable from reusing the old one.
        var serial = ++_issueCount;

        return new JwtTokenPair
        {
            AccessToken = $"access-{user.Id}-{serial}",
            AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenRaw = $"refresh-{user.Id}-{serial}",
            RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(14)
        };
    }

    public string HashRefreshToken(string rawToken) => $"hashed:{rawToken}";
}

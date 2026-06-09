using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PropertyCare.Application.Abstractions;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Infrastructure.Auth;

public sealed class JwtTokenService : IJwtTokenService
{
    /// <summary>Custom claim with the user's tenant id.</summary>
    public const string TenantIdClaim = "tenantId";

    private readonly JwtOptions _options;
    private readonly TimeProvider _clock;

    public JwtTokenService(JwtOptions options, TimeProvider clock)
    {
        _options = options;
        _clock = clock;
    }

    public JwtTokenPair IssueTokens(AppUserEntity user)
    {
        var nowUtc = _clock.GetUtcNow().UtcDateTime;
        var accessExpiresAtUtc = nowUtc.AddMinutes(_options.AccessTokenExpirationMinutes);
        var refreshExpiresAtUtc = nowUtc.AddDays(_options.RefreshTokenExpirationDays);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.Name),
            new(TenantIdClaim, user.TenantId.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: accessExpiresAtUtc,
            signingCredentials: credentials);

        return new JwtTokenPair
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            AccessTokenExpiresAtUtc = accessExpiresAtUtc,
            RefreshTokenRaw = GenerateRefreshToken(),
            RefreshTokenExpiresAtUtc = refreshExpiresAtUtc
        };
    }

    public string HashRefreshToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }

    private static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}

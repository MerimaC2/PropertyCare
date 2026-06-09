using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Auth.Commands.Login;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Application.Modules.Auth.Commands.Refresh;

/// <summary>
/// Exchanges a valid refresh token for a new token pair (refresh token rotation).
/// </summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginCommandDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IJwtTokenService _jwt;
    private readonly TimeProvider _clock;

    public RefreshTokenCommandHandler(IAppDbContext ctx, IJwtTokenService jwt, TimeProvider clock)
    {
        _ctx = ctx;
        _jwt = jwt;
        _clock = clock;
    }

    public async Task<LoginCommandDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var nowUtc = _clock.GetUtcNow().UtcDateTime;
        var tokenHash = _jwt.HashRefreshToken(request.RefreshToken);

        var storedToken = await _ctx.RefreshTokens
            .Include(t => t.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.IsDeleted, ct)
            ?? throw new ConflictException("Refresh token is not valid.");

        if (storedToken.IsRevoked || storedToken.ExpiresAtUtc <= nowUtc)
            throw new ConflictException("Refresh token is expired or revoked.");

        if (storedToken.Fingerprint is not null && storedToken.Fingerprint != request.Fingerprint)
            throw new ConflictException("Refresh token is bound to another device.");

        var user = storedToken.User;
        if (!user.IsActive || user.IsDeleted)
            throw new ConflictException("User is disabled.");

        // Rotate: revoke the used token and issue a fresh pair.
        storedToken.IsRevoked = true;
        storedToken.RevokedAtUtc = nowUtc;

        var tokenPair = _jwt.IssueTokens(user);

        _ctx.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = user.Id,
            TokenHash = _jwt.HashRefreshToken(tokenPair.RefreshTokenRaw),
            ExpiresAtUtc = tokenPair.RefreshTokenExpiresAtUtc,
            Fingerprint = request.Fingerprint,
            IsRevoked = false,
            CreatedAtUtc = nowUtc
        });
        await _ctx.SaveChangesAsync(ct);

        return new LoginCommandDto
        {
            AccessToken = tokenPair.AccessToken,
            RefreshToken = tokenPair.RefreshTokenRaw,
            AccessTokenExpiresAtUtc = tokenPair.AccessTokenExpiresAtUtc.ToString("O"),
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.Name
        };
    }
}

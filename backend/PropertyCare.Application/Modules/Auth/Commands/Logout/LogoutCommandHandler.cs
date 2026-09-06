using PropertyCare.Application.Abstractions;

namespace PropertyCare.Application.Modules.Auth.Commands.Logout;

/// <summary>Revokes the given refresh token so it can no longer be used.</summary>
public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IJwtTokenService _jwt;
    private readonly TimeProvider _clock;

    public LogoutCommandHandler(IAppDbContext ctx, IJwtTokenService jwt, TimeProvider clock)
    {
        _ctx = ctx;
        _jwt = jwt;
        _clock = clock;
    }

    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var tokenHash = _jwt.HashRefreshToken(request.RefreshToken);

        // Logging out carries no access token, so like login and refresh it runs without a tenant
        // and has to look past the global filter. The token hash is what identifies the row.
        var storedToken = await _ctx.RefreshTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.IsDeleted, ct);

        // Logout is idempotent - an unknown token is silently ignored.
        if (storedToken is null || storedToken.IsRevoked)
            return;

        storedToken.IsRevoked = true;
        storedToken.RevokedAtUtc = _clock.GetUtcNow().UtcDateTime;
        await _ctx.SaveChangesAsync(ct);
    }
}

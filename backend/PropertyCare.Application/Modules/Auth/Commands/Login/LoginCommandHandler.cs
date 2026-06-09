using Microsoft.AspNetCore.Identity;
using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Application.Modules.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher<AppUserEntity> _hasher;
    private readonly TimeProvider _clock;

    public LoginCommandHandler(
        IAppDbContext ctx,
        IJwtTokenService jwt,
        IPasswordHasher<AppUserEntity> hasher,
        TimeProvider clock)
    {
        _ctx = ctx;
        _jwt = jwt;
        _hasher = hasher;
        _clock = clock;
    }

    public async Task<LoginCommandDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _ctx.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive && !u.IsDeleted, ct)
            ?? throw new NotFoundException("User not found or disabled.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new ConflictException("Invalid email or password.");

        var tokenPair = _jwt.IssueTokens(user);

        _ctx.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = user.Id,
            TokenHash = _jwt.HashRefreshToken(tokenPair.RefreshTokenRaw),
            ExpiresAtUtc = tokenPair.RefreshTokenExpiresAtUtc,
            Fingerprint = request.Fingerprint,
            IsRevoked = false,
            CreatedAtUtc = _clock.GetUtcNow().UtcDateTime
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

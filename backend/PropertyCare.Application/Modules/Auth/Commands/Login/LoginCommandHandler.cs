using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Application.Modules.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandDto>
{
    /// <summary>
    /// The one answer given for an unknown account, a disabled account and a wrong password alike.
    /// Anything more specific would let a caller enumerate which e-mail addresses exist.
    /// </summary>
    private const string InvalidCredentials = "Invalid email or password.";

    private readonly IAppDbContext _ctx;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher<AppUserEntity> _hasher;
    private readonly TimeProvider _clock;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IAppDbContext ctx,
        IJwtTokenService jwt,
        IPasswordHasher<AppUserEntity> hasher,
        TimeProvider clock,
        ILogger<LoginCommandHandler> logger)
    {
        _ctx = ctx;
        _jwt = jwt;
        _hasher = hasher;
        _clock = clock;
        _logger = logger;
    }

    public async Task<LoginCommandDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Login runs before a tenant is known, so it is one of the few places allowed past the
        // global tenant filter. The e-mail is unique across the whole database.
        var user = await _ctx.Users
            .IgnoreQueryFilters()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, ct);

        if (user is null)
        {
            // Hash anyway, so an unknown address does not answer measurably faster than a wrong
            // password and give the same information away through timing.
            _hasher.HashPassword(new AppUserEntity(), request.Password);

            _logger.LogWarning("Login rejected: no account for {Email}.", email);
            throw new UnauthorizedException(InvalidCredentials);
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login rejected: account {UserId} is disabled.", user.Id);
            throw new UnauthorizedException(InvalidCredentials);
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Login rejected: wrong password for account {UserId}.", user.Id);
            throw new UnauthorizedException(InvalidCredentials);
        }

        var tokenPair = _jwt.IssueTokens(user);

        _ctx.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = user.Id,
            TokenHash = _jwt.HashRefreshToken(tokenPair.RefreshTokenRaw),
            ExpiresAtUtc = tokenPair.RefreshTokenExpiresAtUtc,
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

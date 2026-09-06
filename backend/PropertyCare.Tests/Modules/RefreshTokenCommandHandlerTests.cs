using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Auth.Commands.Refresh;
using PropertyCare.Domain.Entities.Identity;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

/// <summary>
/// The device fingerprint was removed from the refresh flow because nothing ever set it.
/// These tests pin down what actually protects the flow: rotation and revocation.
/// </summary>
public class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidToken_RevokesTheUsedTokenAndIssuesANewOne()
    {
        await using var ctx = TestDbContextFactory.Create();
        var user = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        var stored = AddRefreshToken(ctx, user.Id, "raw-token");

        var result = await BuildHandler(ctx).Handle(
            new RefreshTokenCommand { RefreshToken = "raw-token" }, CancellationToken.None);

        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.NotEqual("raw-token", result.RefreshToken);

        var tokens = await ctx.RefreshTokens.ToListAsync();
        Assert.Equal(2, tokens.Count);
        Assert.True(tokens.Single(t => t.Id == stored.Id).IsRevoked);
        Assert.Single(tokens, t => !t.IsRevoked);
    }

    [Fact]
    public async Task Handle_TokenUsedTwice_ThrowsConflictException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var user = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        AddRefreshToken(ctx, user.Id, "raw-token");
        var handler = BuildHandler(ctx);

        await handler.Handle(
            new RefreshTokenCommand { RefreshToken = "raw-token" }, CancellationToken.None);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(
                new RefreshTokenCommand { RefreshToken = "raw-token" }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExpiredToken_ThrowsConflictException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var user = TestData.AddUser(ctx, roleId: 3, email: "reporter@test.ba");
        AddRefreshToken(ctx, user.Id, "raw-token", expiresAtUtc: DateTime.UtcNow.AddDays(-1));

        await Assert.ThrowsAsync<ConflictException>(
            () => BuildHandler(ctx).Handle(
                new RefreshTokenCommand { RefreshToken = "raw-token" }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnknownToken_ThrowsConflictException()
    {
        await using var ctx = TestDbContextFactory.Create();

        await Assert.ThrowsAsync<ConflictException>(
            () => BuildHandler(ctx).Handle(
                new RefreshTokenCommand { RefreshToken = "never-issued" }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DisabledUser_ThrowsConflictException()
    {
        await using var ctx = TestDbContextFactory.Create();
        var user = TestData.AddUser(ctx, roleId: 3, email: "disabled@test.ba", isActive: false);
        AddRefreshToken(ctx, user.Id, "raw-token");

        await Assert.ThrowsAsync<ConflictException>(
            () => BuildHandler(ctx).Handle(
                new RefreshTokenCommand { RefreshToken = "raw-token" }, CancellationToken.None));
    }

    private static RefreshTokenCommandHandler BuildHandler(DatabaseContext ctx)
        => new(ctx, new FakeJwtTokenService(), TimeProvider.System);

    private static RefreshTokenEntity AddRefreshToken(
        DatabaseContext ctx, int userId, string rawToken, DateTime? expiresAtUtc = null)
    {
        var token = new RefreshTokenEntity
        {
            UserId = userId,
            TokenHash = $"hashed:{rawToken}",
            ExpiresAtUtc = expiresAtUtc ?? DateTime.UtcNow.AddDays(14),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        ctx.RefreshTokens.Add(token);
        ctx.SaveChanges();
        return token;
    }
}

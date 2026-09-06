using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Application.Modules.Auth.Commands.Login;
using PropertyCare.Domain.Entities.Identity;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Tests.Common;

namespace PropertyCare.Tests.Modules;

public class LoginCommandHandlerTests
{
    private const string CorrectPassword = "Correct123!";

    [Fact]
    public async Task Handle_CorrectCredentials_ReturnsTokens()
    {
        await using var ctx = TestDbContextFactory.Create();
        AddUserWithPassword(ctx, "user@test.ba", CorrectPassword);
        var handler = BuildHandler(ctx);

        var result = await handler.Handle(
            new LoginCommand { Email = "user@test.ba", Password = CorrectPassword },
            CancellationToken.None);

        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.Equal("user@test.ba", result.Email);
        Assert.Single(await ctx.RefreshTokens.ToListAsync());
    }

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsUnauthorized()
    {
        await using var ctx = TestDbContextFactory.Create();
        var handler = BuildHandler(ctx);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => handler.Handle(
                new LoginCommand { Email = "nobody@test.ba", Password = CorrectPassword },
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        await using var ctx = TestDbContextFactory.Create();
        AddUserWithPassword(ctx, "user@test.ba", CorrectPassword);
        var handler = BuildHandler(ctx);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => handler.Handle(
                new LoginCommand { Email = "user@test.ba", Password = "WrongPassword1!" },
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DisabledAccount_ThrowsUnauthorized()
    {
        await using var ctx = TestDbContextFactory.Create();
        AddUserWithPassword(ctx, "disabled@test.ba", CorrectPassword, isActive: false);
        var handler = BuildHandler(ctx);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => handler.Handle(
                new LoginCommand { Email = "disabled@test.ba", Password = CorrectPassword },
                CancellationToken.None));
    }

    /// <summary>
    /// The point of the fix: all three failures have to be indistinguishable from the outside,
    /// otherwise comparing responses tells an attacker which addresses exist.
    /// </summary>
    [Fact]
    public async Task Handle_EveryFailureReason_GivesTheSameMessage()
    {
        await using var ctx = TestDbContextFactory.Create();
        AddUserWithPassword(ctx, "user@test.ba", CorrectPassword);
        AddUserWithPassword(ctx, "disabled@test.ba", CorrectPassword, isActive: false);
        var handler = BuildHandler(ctx);

        var messages = new List<string>();
        foreach (var command in new[]
        {
            new LoginCommand { Email = "nobody@test.ba", Password = CorrectPassword },
            new LoginCommand { Email = "user@test.ba", Password = "WrongPassword1!" },
            new LoginCommand { Email = "disabled@test.ba", Password = CorrectPassword }
        })
        {
            var ex = await Assert.ThrowsAsync<UnauthorizedException>(
                () => handler.Handle(command, CancellationToken.None));
            messages.Add(ex.Message);
        }

        Assert.Single(messages.Distinct());
        Assert.Equal("Invalid email or password.", messages[0]);
    }

    private static LoginCommandHandler BuildHandler(DatabaseContext ctx)
        => new(
            ctx,
            new FakeJwtTokenService(),
            new PasswordHasher<AppUserEntity>(),
            TimeProvider.System,
            NullLogger<LoginCommandHandler>.Instance);

    private static AppUserEntity AddUserWithPassword(
        DatabaseContext ctx, string email, string password, bool isActive = true)
    {
        var user = new AppUserEntity
        {
            TenantId = 1,
            RoleId = 3,
            FirstName = "Test",
            LastName = "User",
            Email = email,
            IsActive = isActive
        };
        user.PasswordHash = new PasswordHasher<AppUserEntity>().HashPassword(user, password);

        ctx.Users.Add(user);
        ctx.SaveChanges();
        return user;
    }
}

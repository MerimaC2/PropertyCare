using Microsoft.EntityFrameworkCore;
using PropertyCare.Infrastructure.Database;

namespace PropertyCare.Tests.Common;

/// <summary>Creates an isolated in-memory database per test, pre-filled with static lookup data.</summary>
public static class TestDbContextFactory
{
    public static DatabaseContext Create()
    {
        var context = new DatabaseContext(BuildOptions(), TimeProvider.System);

        // EnsureCreated also applies the HasData seed (tenant, roles, statuses, priorities...).
        context.Database.EnsureCreated();

        return context;
    }

    /// <summary>Same database, but with a save that can be made to fail mid-test.</summary>
    public static FailingDbContext CreateFailing()
    {
        var context = new FailingDbContext(BuildOptions());
        context.Database.EnsureCreated();

        return context;
    }

    private static DbContextOptions<DatabaseContext> BuildOptions()
        => new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"PropertyCareTests_{Guid.NewGuid()}")
            .Options;
}

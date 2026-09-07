using Microsoft.EntityFrameworkCore;
using PropertyCare.Infrastructure.Database;

namespace PropertyCare.Tests.Common;

/// <summary>Creates an isolated in-memory database per test, pre-filled with static lookup data.</summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Context signed in as the given tenant. Tenant 1 by default, because that is the tenant the
    /// static seed and <see cref="TestData"/> use.
    /// </summary>
    public static DatabaseContext Create(int? tenantId = 1, string? databaseName = null)
    {
        var context = new DatabaseContext(
            BuildOptions(databaseName ?? NewDatabaseName()),
            TimeProvider.System,
            new FakeCurrentUser { TenantId = tenantId });

        // EnsureCreated also applies the HasData seed (tenant, roles, statuses, priorities...).
        context.Database.EnsureCreated();

        return context;
    }

    /// <summary>Same database, but with a save that can be made to fail mid-test.</summary>
    public static FailingDbContext CreateFailing()
    {
        var context = new FailingDbContext(
            BuildOptions(NewDatabaseName()),
            new FakeCurrentUser { TenantId = 1 });
        context.Database.EnsureCreated();

        return context;
    }

    /// <summary>
    /// Name to share between two contexts so the same store can be read as two different tenants -
    /// that is the only way to prove that one tenant cannot see the other's rows.
    /// </summary>
    public static string NewDatabaseName() => $"PropertyCareTests_{Guid.NewGuid()}";

    private static DbContextOptions<DatabaseContext> BuildOptions(string databaseName)
        => new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
}

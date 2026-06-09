using Microsoft.EntityFrameworkCore;
using PropertyCare.Infrastructure.Database;

namespace PropertyCare.Tests.Common;

/// <summary>Creates an isolated in-memory database per test, pre-filled with static lookup data.</summary>
public static class TestDbContextFactory
{
    public static DatabaseContext Create()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"PropertyCareTests_{Guid.NewGuid()}")
            .Options;

        var context = new DatabaseContext(options, TimeProvider.System);

        // EnsureCreated also applies the HasData seed (tenant, roles, statuses, priorities...).
        context.Database.EnsureCreated();

        return context;
    }
}

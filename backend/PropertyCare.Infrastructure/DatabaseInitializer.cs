using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Infrastructure.Database.Seeders;

namespace PropertyCare.Infrastructure;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider, IHostEnvironment env)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();

        if (env.IsEnvironment("Test"))
        {
            // Tests always start from a fresh database.
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            await seeder.SeedDynamicDataAsync();
        }
        else if (env.IsDevelopment())
        {
            await context.Database.MigrateAsync();
            await seeder.SeedDynamicDataAsync();
        }
        else
        {
            // Staging/Production: migrations only, no demo data.
            await context.Database.MigrateAsync();
        }
    }
}

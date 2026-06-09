namespace PropertyCare.Infrastructure.Database.Seeders;

public interface IDatabaseSeeder
{
    /// <summary>Seeds demo data (users, facilities, requests) when the database is empty.</summary>
    Task SeedDynamicDataAsync(CancellationToken ct = default);
}

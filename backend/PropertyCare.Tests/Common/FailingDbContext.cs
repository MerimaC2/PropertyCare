using Microsoft.EntityFrameworkCore;
using PropertyCare.Application.Abstractions;
using PropertyCare.Infrastructure.Database;

namespace PropertyCare.Tests.Common;

/// <summary>
/// Database context whose save can be switched to fail on demand, so compensating cleanup after a
/// failed write can be tested.
/// </summary>
public sealed class FailingDbContext : DatabaseContext
{
    public FailingDbContext(DbContextOptions<DatabaseContext> options, IAppCurrentUser currentUser)
        : base(options, TimeProvider.System, currentUser)
    {
    }

    /// <summary>When true, every save throws instead of writing.</summary>
    public bool FailOnSave { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        => FailOnSave
            ? throw new InvalidOperationException("Simulated database failure.")
            : base.SaveChangesAsync(ct);
}

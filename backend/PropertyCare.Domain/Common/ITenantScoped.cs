namespace PropertyCare.Domain.Common;

/// <summary>
/// Marks an entity whose rows belong to exactly one tenant. Every such entity gets a global
/// query filter in <c>DatabaseContext.OnModelCreating</c>, so a query can never reach across
/// tenants even if a handler forgets to filter.
/// </summary>
public interface ITenantScoped
{
    int TenantId { get; }
}

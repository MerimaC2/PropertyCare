using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Buildings.Queries.NameExists;

public sealed class BuildingNameExistsQueryHandler
    : IRequestHandler<BuildingNameExistsQuery, bool>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public BuildingNameExistsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(BuildingNameExistsQuery request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var name = (request.Name ?? string.Empty).Trim();
        if (name.Length == 0)
            return false;

        return await _ctx.Buildings.AnyAsync(
            b => b.TenantId == tenantId
                && !b.IsDeleted
                && b.Name == name
                && (request.ExcludeId == null || b.Id != request.ExcludeId),
            ct);
    }
}

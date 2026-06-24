using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Units.Commands.Delete;

public sealed class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public DeleteUnitCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteUnitCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var unit = await _ctx.Units.FirstOrDefaultAsync(
            u => u.Id == request.Id && u.TenantId == tenantId && !u.IsDeleted, ct)
            ?? throw new NotFoundException("Unit not found.");

        var hasAssets = await _ctx.Assets
            .AnyAsync(a => a.UnitId == unit.Id && !a.IsDeleted, ct);
        if (hasAssets)
            throw new ConflictException("Cannot delete a unit that still has assets. Remove its assets first.");

        unit.IsDeleted = true;
        await _ctx.SaveChangesAsync(ct);
    }
}

using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Buildings.Commands.Delete;

public sealed class DeleteBuildingCommandHandler : IRequestHandler<DeleteBuildingCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public DeleteBuildingCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteBuildingCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var building = await _ctx.Buildings.FirstOrDefaultAsync(
            b => b.Id == request.Id && b.TenantId == tenantId && !b.IsDeleted, ct)
            ?? throw new NotFoundException("Building not found.");

        var hasUnits = await _ctx.Units
            .AnyAsync(u => u.BuildingId == building.Id && !u.IsDeleted, ct);
        if (hasUnits)
            throw new ConflictException("Cannot delete a building that still has units. Remove its units first.");

        building.IsDeleted = true;
        await _ctx.SaveChangesAsync(ct);
    }
}

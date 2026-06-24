using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Buildings.Commands.Update;

public sealed class UpdateBuildingCommandHandler : IRequestHandler<UpdateBuildingCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public UpdateBuildingCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateBuildingCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var building = await _ctx.Buildings.FirstOrDefaultAsync(
            b => b.Id == request.Id && b.TenantId == tenantId && !b.IsDeleted, ct)
            ?? throw new NotFoundException("Building not found.");

        var typeExists = await _ctx.BuildingTypes
            .AnyAsync(t => t.Id == request.BuildingTypeId && !t.IsDeleted, ct);
        if (!typeExists)
            throw new ValidationException("Building type not found.");

        building.Name = request.Name.Trim();
        building.Address = request.Address?.Trim();
        building.BuildingTypeId = request.BuildingTypeId;
        building.Latitude = request.Latitude;
        building.Longitude = request.Longitude;

        await _ctx.SaveChangesAsync(ct);
    }
}

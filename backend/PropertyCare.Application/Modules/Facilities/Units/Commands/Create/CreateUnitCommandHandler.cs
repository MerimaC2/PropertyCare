using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Application.Modules.Facilities.Units.Commands.Create;

public sealed class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CreateUnitCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateUnitCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var buildingExists = await _ctx.Buildings.AnyAsync(
            b => b.Id == request.BuildingId && b.TenantId == tenantId && !b.IsDeleted, ct);
        if (!buildingExists)
            throw new ValidationException("Building not found.");

        var entity = new UnitEntity
        {
            TenantId = tenantId,
            BuildingId = request.BuildingId,
            Label = request.Label.Trim()
        };
        _ctx.Units.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}

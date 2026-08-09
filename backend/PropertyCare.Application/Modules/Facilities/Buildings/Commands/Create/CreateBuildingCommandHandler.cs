using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Application.Modules.Facilities.Buildings.Commands.Create;

public sealed class CreateBuildingCommandHandler : IRequestHandler<CreateBuildingCommand, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CreateBuildingCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateBuildingCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var typeExists = await _ctx.BuildingTypes
            .AnyAsync(t => t.Id == request.BuildingTypeId && !t.IsDeleted, ct);
        if (!typeExists)
            throw new ValidationException("Building type not found.");

        var entity = new BuildingEntity
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Address = request.Address?.Trim(),
            BuildingTypeId = request.BuildingTypeId,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };
        _ctx.Buildings.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}

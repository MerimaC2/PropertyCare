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

        // The type has to belong to this tenant too, not just exist somewhere in the table.
        var typeExists = await _ctx.BuildingTypes.AnyAsync(
            t => t.Id == request.BuildingTypeId && t.TenantId == tenantId && !t.IsDeleted, ct);
        if (!typeExists)
            throw new ValidationException("Building type not found.");

        // The frontend async validator is only a convenience; this is the check that counts,
        // and the unique index behind it catches anything that slips through a race.
        var nameNormalized = BuildingEntity.NormalizeName(request.Name);
        var nameTaken = await _ctx.Buildings.AnyAsync(
            b => b.TenantId == tenantId && b.NameNormalized == nameNormalized && !b.IsDeleted, ct);
        if (nameTaken)
            throw new ConflictException("A building with this name already exists.");

        var entity = new BuildingEntity
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            NameNormalized = nameNormalized,
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

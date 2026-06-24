using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Application.Modules.Facilities.Assets.Commands.Create;

public sealed class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CreateAssetCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateAssetCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var unitExists = await _ctx.Units.AnyAsync(
            u => u.Id == request.UnitId && u.TenantId == tenantId && !u.IsDeleted, ct);
        if (!unitExists)
            throw new ValidationException("Unit not found.");

        var typeExists = await _ctx.AssetTypes.AnyAsync(
            t => t.Id == request.AssetTypeId && t.TenantId == tenantId && !t.IsDeleted, ct);
        if (!typeExists)
            throw new ValidationException("Asset type not found.");

        var entity = new AssetEntity
        {
            TenantId = tenantId,
            UnitId = request.UnitId,
            AssetTypeId = request.AssetTypeId,
            Name = request.Name.Trim()
        };
        _ctx.Assets.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}

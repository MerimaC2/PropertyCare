using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Delete;

public sealed class DeleteAssetTypeCommandHandler : IRequestHandler<DeleteAssetTypeCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public DeleteAssetTypeCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteAssetTypeCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var assetType = await _ctx.AssetTypes.FirstOrDefaultAsync(
            t => t.Id == request.Id && t.TenantId == tenantId && !t.IsDeleted, ct)
            ?? throw new NotFoundException("Asset type not found.");

        var inUse = await _ctx.Assets
            .AnyAsync(a => a.AssetTypeId == assetType.Id && !a.IsDeleted, ct);
        if (inUse)
            throw new ConflictException("Cannot delete an asset type that is still used by assets.");

        assetType.IsDeleted = true;
        await _ctx.SaveChangesAsync(ct);
    }
}

using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Assets.Commands.Update;

public sealed class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public UpdateAssetCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateAssetCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var asset = await _ctx.Assets.FirstOrDefaultAsync(
            a => a.Id == request.Id && a.TenantId == tenantId && !a.IsDeleted, ct)
            ?? throw new NotFoundException("Asset not found.");

        var typeExists = await _ctx.AssetTypes.AnyAsync(
            t => t.Id == request.AssetTypeId && t.TenantId == tenantId && !t.IsDeleted, ct);
        if (!typeExists)
            throw new ValidationException("Asset type not found.");

        asset.Name = request.Name.Trim();
        asset.AssetTypeId = request.AssetTypeId;
        await _ctx.SaveChangesAsync(ct);
    }
}

using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Update;

public sealed class UpdateAssetTypeCommandHandler : IRequestHandler<UpdateAssetTypeCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public UpdateAssetTypeCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateAssetTypeCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var assetType = await _ctx.AssetTypes.FirstOrDefaultAsync(
            t => t.Id == request.Id && t.TenantId == tenantId && !t.IsDeleted, ct)
            ?? throw new NotFoundException("Asset type not found.");

        assetType.Name = request.Name.Trim();
        assetType.DefaultSlaHours = request.DefaultSlaHours;
        await _ctx.SaveChangesAsync(ct);
    }
}

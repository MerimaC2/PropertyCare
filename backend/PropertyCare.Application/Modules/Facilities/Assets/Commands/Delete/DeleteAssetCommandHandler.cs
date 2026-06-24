using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Assets.Commands.Delete;

public sealed class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public DeleteAssetCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteAssetCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var asset = await _ctx.Assets.FirstOrDefaultAsync(
            a => a.Id == request.Id && a.TenantId == tenantId && !a.IsDeleted, ct)
            ?? throw new NotFoundException("Asset not found.");

        asset.IsDeleted = true;
        await _ctx.SaveChangesAsync(ct);
    }
}

using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Create;

public sealed class CreateAssetTypeCommandHandler : IRequestHandler<CreateAssetTypeCommand, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CreateAssetTypeCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateAssetTypeCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var entity = new AssetTypeEntity
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            DefaultSlaHours = request.DefaultSlaHours
        };
        _ctx.AssetTypes.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}

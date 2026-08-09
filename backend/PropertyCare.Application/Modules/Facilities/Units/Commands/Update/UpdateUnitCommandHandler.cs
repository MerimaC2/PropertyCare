using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.Facilities.Units.Commands.Update;

public sealed class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public UpdateUnitCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateUnitCommand request, CancellationToken ct)
    {
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        var unit = await _ctx.Units.FirstOrDefaultAsync(
            u => u.Id == request.Id && u.TenantId == tenantId && !u.IsDeleted, ct)
            ?? throw new NotFoundException("Unit not found.");

        unit.Label = request.Label.Trim();
        await _ctx.SaveChangesAsync(ct);
    }
}

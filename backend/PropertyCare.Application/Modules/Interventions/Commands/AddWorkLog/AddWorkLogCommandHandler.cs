using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Application.Modules.Interventions.Commands.AddWorkLog;

public sealed class AddWorkLogCommandHandler : IRequestHandler<AddWorkLogCommand, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public AddWorkLogCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(AddWorkLogCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        // Technician may only log work on their own work orders.
        var owns = await _ctx.WorkOrders.AnyAsync(
            w => w.Id == request.WorkOrderId && w.AssignedToUserId == userId && !w.IsDeleted, ct);
        if (!owns)
            throw new NotFoundException("Work order not found.");

        var log = new WorkLogEntity
        {
            TenantId = tenantId,
            WorkOrderId = request.WorkOrderId,
            Note = request.Note.Trim(),
            MinutesSpent = request.MinutesSpent
        };
        _ctx.WorkLogs.Add(log);
        await _ctx.SaveChangesAsync(ct);

        return log.Id;
    }
}

using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Identity;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Application.Modules.WorkOrders.Commands.Assign;

public sealed class AssignWorkOrderCommandHandler : IRequestHandler<AssignWorkOrderCommand, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public AssignWorkOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(AssignWorkOrderCommand request, CancellationToken ct)
    {
        var adminUserId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");

        // 1. Request must exist and must not be closed.
        var maintenanceRequest = await _ctx.MaintenanceRequests
            .Include(r => r.Status)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId && !r.IsDeleted, ct)
            ?? throw new NotFoundException("Maintenance request not found.");

        if (maintenanceRequest.Status.IsTerminal)
            throw new ConflictException("A closed request cannot be assigned.");

        // 2. A request can have only one active work order at a time.
        var hasActiveWorkOrder = await _ctx.WorkOrders.AnyAsync(
            w => w.RequestId == request.RequestId && !w.IsDeleted && !w.Status.IsTerminal, ct);
        if (hasActiveWorkOrder)
            throw new ConflictException("The request is already assigned to a technician.");

        // 3. The assignee must be an active technician.
        var technician = await _ctx.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == request.AssignedToUserId && !u.IsDeleted, ct)
            ?? throw new NotFoundException("Technician not found.");

        if (!technician.IsActive || technician.Role.Name != UserRoleEntity.Names.Technician)
            throw new ValidationException("The selected user is not an active technician.");

        // 4. Create the work order in the ASSIGNED status.
        var workOrderStatus = await _ctx.WorkOrderStatuses
            .FirstOrDefaultAsync(s => s.Abrv == WorkOrderStatusEntity.Codes.Assigned && !s.IsDeleted, ct)
            ?? throw new NotFoundException("Initial work order status is not configured.");

        var workOrder = new WorkOrderEntity
        {
            TenantId = maintenanceRequest.TenantId,
            RequestId = maintenanceRequest.Id,
            AssignedToUserId = technician.Id,
            StatusId = workOrderStatus.Id,
            Note = request.Note?.Trim()
        };
        _ctx.WorkOrders.Add(workOrder);

        // 5. Move the request to ASSIGNED and record the transition.
        var assignedRequestStatus = await _ctx.RequestStatuses
            .FirstOrDefaultAsync(s => s.Abrv == RequestStatusEntity.Codes.Assigned && !s.IsDeleted, ct)
            ?? throw new NotFoundException("Assigned request status is not configured.");

        if (maintenanceRequest.StatusId != assignedRequestStatus.Id)
        {
            _ctx.RequestStatusHistories.Add(new RequestStatusHistoryEntity
            {
                RequestId = maintenanceRequest.Id,
                FromStatusId = maintenanceRequest.StatusId,
                ToStatusId = assignedRequestStatus.Id,
                ChangedByUserId = adminUserId,
                Note = $"Assigned to {technician.FullName}."
            });
            maintenanceRequest.StatusId = assignedRequestStatus.Id;
        }

        await _ctx.SaveChangesAsync(ct);

        return workOrder.Id;
    }
}

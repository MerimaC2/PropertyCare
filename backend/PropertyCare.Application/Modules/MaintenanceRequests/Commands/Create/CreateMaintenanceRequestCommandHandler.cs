using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Maintenance;
using PropertyCare.Domain.Entities.System;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Commands.Create;

public sealed class CreateMaintenanceRequestCommandHandler
    : IRequestHandler<CreateMaintenanceRequestCommand, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CreateMaintenanceRequestCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateMaintenanceRequestCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");
        var tenantId = _currentUser.TenantId
            ?? throw new ForbiddenException("User has no tenant.");

        // 1. Building must exist.
        var buildingExists = await _ctx.Buildings
            .AnyAsync(b => b.Id == request.BuildingId && !b.IsDeleted, ct);
        if (!buildingExists)
            throw new ValidationException("Building not found.");

        // 2. Unit (when selected) must belong to the selected building.
        if (request.UnitId.HasValue)
        {
            var unitValid = await _ctx.Units.AnyAsync(
                u => u.Id == request.UnitId && u.BuildingId == request.BuildingId && !u.IsDeleted, ct);
            if (!unitValid)
                throw new ValidationException("Unit not found in the selected building.");
        }

        // 3. Asset (when selected) must belong to the selected unit.
        if (request.AssetId.HasValue)
        {
            var assetValid = await _ctx.Assets.AnyAsync(
                a => a.Id == request.AssetId && a.UnitId == request.UnitId && !a.IsDeleted, ct);
            if (!assetValid)
                throw new ValidationException("Asset not found in the selected unit.");
        }

        // 4. Priority must exist.
        var priorityExists = await _ctx.RequestPriorities
            .AnyAsync(p => p.Id == request.PriorityId && !p.IsDeleted, ct);
        if (!priorityExists)
            throw new ValidationException("Priority not found.");

        // 5. New requests always start in the NEW status.
        var newStatus = await _ctx.RequestStatuses
            .FirstOrDefaultAsync(s => s.Abrv == RequestStatusEntity.Codes.New && !s.IsDeleted, ct)
            ?? throw new NotFoundException("Initial request status is not configured.");

        var entity = new MaintenanceRequestEntity
        {
            TenantId = tenantId,
            BuildingId = request.BuildingId,
            UnitId = request.UnitId,
            AssetId = request.AssetId,
            CreatedByUserId = userId,
            PriorityId = request.PriorityId,
            StatusId = newStatus.Id,
            Title = request.Title.Trim(),
            Description = request.Description.Trim()
        };
        _ctx.MaintenanceRequests.Add(entity);

        // 6. Record the initial status transition.
        entity.StatusHistory.Add(new RequestStatusHistoryEntity
        {
            FromStatusId = null,
            ToStatusId = newStatus.Id,
            ChangedByUserId = userId,
            Note = "Request created."
        });

        // 7. Notify the reporter that their request was received.
        _ctx.Notifications.Add(new NotificationEntity
        {
            TenantId = tenantId,
            UserId = userId,
            Type = NotificationType.RequestSubmitted,
            Title = "Request submitted",
            Message = $"Your request \"{entity.Title}\" was submitted and is awaiting triage."
        });

        await _ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}

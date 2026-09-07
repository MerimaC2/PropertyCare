using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Application.Modules.MaintenanceRequests;

/// <summary>
/// One place that decides who may look at a maintenance request and its attachments, so the list
/// and the file-download endpoints cannot drift apart.
/// </summary>
internal static class RequestAccess
{
    /// <summary>
    /// True when the current user may view <paramref name="requestId"/>: a reporter only their own
    /// request, an administrator any request of their tenant, a technician any request tied to one
    /// of their work orders.
    /// </summary>
    public static async Task<bool> CanViewAsync(
        IAppDbContext ctx, IAppCurrentUser currentUser, int requestId, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");

        var request = ctx.MaintenanceRequests.AsNoTracking()
            .Where(r => r.Id == requestId && !r.IsDeleted);

        return currentUser.Role switch
        {
            UserRoleEntity.Names.Administrator => await request.AnyAsync(
                r => r.TenantId == currentUser.TenantId, ct),

            UserRoleEntity.Names.Technician => await request.AnyAsync(
                r => r.WorkOrders.Any(w => w.AssignedToUserId == userId && !w.IsDeleted), ct),

            _ => await request.AnyAsync(r => r.CreatedByUserId == userId, ct)
        };
    }
}

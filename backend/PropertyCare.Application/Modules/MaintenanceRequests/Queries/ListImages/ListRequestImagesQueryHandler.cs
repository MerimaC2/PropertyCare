using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListImages;

public sealed class ListRequestImagesQueryHandler
    : IRequestHandler<ListRequestImagesQuery, IReadOnlyList<RequestImageDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ListRequestImagesQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<RequestImageDto>> Handle(
        ListRequestImagesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");

        var owns = await _ctx.MaintenanceRequests.AnyAsync(
            r => r.Id == request.RequestId && r.CreatedByUserId == userId && !r.IsDeleted, ct);
        if (!owns)
            throw new NotFoundException("Maintenance request not found.");

        return await _ctx.RequestImages.AsNoTracking()
            .Where(i => i.RequestId == request.RequestId && !i.IsDeleted)
            .OrderBy(i => i.CreatedAtUtc)
            .Select(i => new RequestImageDto
            {
                Id = i.Id,
                FileName = i.FileName,
                ContentType = i.ContentType,
                SizeBytes = i.SizeBytes,
                Url = "/" + i.RelativePath
            })
            .ToListAsync(ct);
    }
}

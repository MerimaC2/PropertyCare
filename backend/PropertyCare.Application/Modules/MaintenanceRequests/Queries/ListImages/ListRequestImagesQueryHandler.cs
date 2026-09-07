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
        // Reporter, administrator and technician each see a different slice of the requests.
        var canView = await RequestAccess.CanViewAsync(_ctx, _currentUser, request.RequestId, ct);
        if (!canView)
            throw new NotFoundException("Maintenance request not found.");

        var images = await _ctx.RequestImages.AsNoTracking()
            .Where(i => i.RequestId == request.RequestId && !i.IsDeleted)
            .OrderBy(i => i.CreatedAtUtc)
            .Select(i => new
            {
                i.Id,
                i.FileName,
                i.ContentType,
                i.SizeBytes
            })
            .ToListAsync(ct);

        return images
            .Select(i => new RequestImageDto
            {
                Id = i.Id,
                FileName = i.FileName,
                ContentType = i.ContentType,
                SizeBytes = i.SizeBytes,
                Url = RequestImageDto.BuildContentUrl(request.RequestId, i.Id)
            })
            .ToList();
    }
}

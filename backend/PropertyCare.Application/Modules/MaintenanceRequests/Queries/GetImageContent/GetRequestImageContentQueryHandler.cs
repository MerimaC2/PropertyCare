using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.GetImageContent;

public sealed class GetRequestImageContentQueryHandler
    : IRequestHandler<GetRequestImageContentQuery, RequestImageContentDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;
    private readonly IFileStorageService _storage;

    public GetRequestImageContentQueryHandler(
        IAppDbContext ctx, IAppCurrentUser currentUser, IFileStorageService storage)
    {
        _ctx = ctx;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task<RequestImageContentDto> Handle(
        GetRequestImageContentQuery request, CancellationToken ct)
    {
        // Authorization is re-checked on the file itself, not only on the metadata listing.
        var canView = await RequestAccess.CanViewAsync(_ctx, _currentUser, request.RequestId, ct);
        if (!canView)
            throw new NotFoundException("Maintenance request not found.");

        var image = await _ctx.RequestImages.AsNoTracking()
            .FirstOrDefaultAsync(
                i => i.Id == request.ImageId && i.RequestId == request.RequestId && !i.IsDeleted, ct)
            ?? throw new NotFoundException("Image not found.");

        var content = _storage.OpenRead(image.RelativePath)
            ?? throw new NotFoundException("Image file is missing from storage.");

        return new RequestImageContentDto
        {
            Content = content,
            ContentType = image.ContentType,
            FileName = image.FileName
        };
    }
}

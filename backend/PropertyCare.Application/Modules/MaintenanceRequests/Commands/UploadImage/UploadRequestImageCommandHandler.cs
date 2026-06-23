using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common.Exceptions;
using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Commands.UploadImage;

public sealed class UploadRequestImageCommandHandler
    : IRequestHandler<UploadRequestImageCommand, RequestImageDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;
    private readonly IFileStorageService _storage;

    public UploadRequestImageCommandHandler(
        IAppDbContext ctx, IAppCurrentUser currentUser, IFileStorageService storage)
    {
        _ctx = ctx;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task<RequestImageDto> Handle(UploadRequestImageCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException("User is not authenticated.");

        // Reporter may only attach images to their own request.
        var owns = await _ctx.MaintenanceRequests.AnyAsync(
            r => r.Id == request.RequestId && r.CreatedByUserId == userId && !r.IsDeleted, ct);
        if (!owns)
            throw new NotFoundException("Maintenance request not found.");

        var relativePath = await _storage.SaveAsync(
            $"uploads/{request.RequestId}", request.FileName, request.Content, ct);

        var entity = new RequestImageEntity
        {
            RequestId = request.RequestId,
            FileName = request.FileName.Trim(),
            ContentType = request.ContentType,
            RelativePath = relativePath,
            SizeBytes = request.SizeBytes
        };
        _ctx.RequestImages.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return new RequestImageDto
        {
            Id = entity.Id,
            FileName = entity.FileName,
            ContentType = entity.ContentType,
            SizeBytes = entity.SizeBytes,
            Url = "/" + entity.RelativePath
        };
    }
}

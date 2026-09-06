using PropertyCare.Application.Abstractions;
using PropertyCare.Application.Common;
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

        // The declared content type and the file name are both client-supplied, so the real format
        // is read from the file itself and decides both the stored type and the extension.
        var contentType = await ImageSignature.DetectAsync(request.Content, ct);
        if (contentType is null || !contentType.Equals(request.ContentType, StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("The file is not a valid JPEG, PNG or WebP image.");

        var extension = RequestImageEntity.Constraints.ExtensionByContentType[contentType];

        var relativePath = await _storage.SaveAsync(
            $"uploads/{request.RequestId}", extension, request.Content, ct);

        var entity = new RequestImageEntity
        {
            RequestId = request.RequestId,
            FileName = request.FileName.Trim(),
            ContentType = contentType,
            RelativePath = relativePath,
            SizeBytes = request.SizeBytes
        };
        _ctx.RequestImages.Add(entity);

        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch
        {
            // The file is already on disk; without its database row nothing would ever reference it.
            _storage.Delete(relativePath);
            throw;
        }

        return new RequestImageDto
        {
            Id = entity.Id,
            FileName = entity.FileName,
            ContentType = entity.ContentType,
            SizeBytes = entity.SizeBytes,
            Url = RequestImageDto.BuildContentUrl(entity.RequestId, entity.Id)
        };
    }
}

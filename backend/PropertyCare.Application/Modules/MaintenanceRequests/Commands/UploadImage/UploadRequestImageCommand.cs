namespace PropertyCare.Application.Modules.MaintenanceRequests.Commands.UploadImage;

public sealed class UploadRequestImageCommand : IRequest<RequestImageDto>
{
    public int RequestId { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }

    /// <summary>Readable stream with the file contents (provided by the controller from IFormFile).</summary>
    public Stream Content { get; set; } = null!;
}

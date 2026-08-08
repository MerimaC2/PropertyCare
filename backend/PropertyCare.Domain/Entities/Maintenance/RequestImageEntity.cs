using PropertyCare.Domain.Common;

namespace PropertyCare.Domain.Entities.Maintenance;

/// <summary>A photo attached to a maintenance request, stored on disk under wwwroot/uploads.</summary>
public sealed class RequestImageEntity : BaseEntity
{
    public int RequestId { get; set; }
    public MaintenanceRequestEntity Request { get; set; } = null!;

    /// <summary>Original file name as uploaded by the user.</summary>
    public string FileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    /// <summary>Path relative to wwwroot, e.g. "uploads/12/{guid}.jpg".</summary>
    public string RelativePath { get; set; } = null!;

    public long SizeBytes { get; set; }

    public static class Constraints
    {
        public const int FileNameMaxLength = 255;
        public const int ContentTypeMaxLength = 100;
        public const int RelativePathMaxLength = 400;

        public const long MaxSizeBytes = 5 * 1024 * 1024; // 5 MB
        public static readonly string[] AllowedContentTypes =
            ["image/jpeg", "image/png", "image/webp"];
    }
}

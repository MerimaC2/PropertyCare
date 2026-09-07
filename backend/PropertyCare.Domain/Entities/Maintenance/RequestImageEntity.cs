using PropertyCare.Domain.Common;

namespace PropertyCare.Domain.Entities.Maintenance;

/// <summary>A photo attached to a maintenance request, stored outside the web root.</summary>
public sealed class RequestImageEntity : BaseEntity
{
    public int RequestId { get; set; }
    public MaintenanceRequestEntity Request { get; set; } = null!;

    /// <summary>Original file name as uploaded by the user.</summary>
    public string FileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    /// <summary>Path relative to the storage root, e.g. "uploads/12/{guid}.jpg".</summary>
    public string RelativePath { get; set; } = null!;

    public long SizeBytes { get; set; }

    public static class Constraints
    {
        public const int FileNameMaxLength = 255;
        public const int ContentTypeMaxLength = 100;
        public const int RelativePathMaxLength = 400;

        public const long MaxSizeBytes = 5 * 1024 * 1024; // 5 MB

        /// <summary>
        /// The only accepted image types, each mapped to the extension the server stores it under.
        /// The extension is never taken from the client-supplied file name.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, string> ExtensionByContentType =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["image/jpeg"] = ".jpg",
                ["image/png"] = ".png",
                ["image/webp"] = ".webp"
            };

        public static readonly string[] AllowedContentTypes =
            ["image/jpeg", "image/png", "image/webp"];
    }
}

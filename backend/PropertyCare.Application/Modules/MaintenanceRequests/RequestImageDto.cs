namespace PropertyCare.Application.Modules.MaintenanceRequests;

/// <summary>An image attached to a maintenance request, with the API URL that serves its content.</summary>
public sealed class RequestImageDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }

    /// <summary>
    /// Authorized API URL of the file itself, e.g. "/api/maintenance-requests/12/images/3/content".
    /// Attachments are not served as static files, so this always requires a bearer token.
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>Builds the URL of the action that streams an image back to an authorized caller.</summary>
    public static string BuildContentUrl(int requestId, int imageId)
        => $"/api/maintenance-requests/{requestId}/images/{imageId}/content";
}

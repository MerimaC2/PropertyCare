namespace PropertyCare.Application.Modules.MaintenanceRequests;

/// <summary>An image attached to a maintenance request, with a web-root-relative URL.</summary>
public sealed class RequestImageDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }

    /// <summary>URL relative to the API origin, e.g. "/uploads/12/{guid}.jpg".</summary>
    public string Url { get; set; } = null!;
}

namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.GetImageContent;

/// <summary>An open read stream over a stored attachment, plus what the caller needs to render it.</summary>
public sealed class RequestImageContentDto
{
    public Stream Content { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public string FileName { get; init; } = null!;
}

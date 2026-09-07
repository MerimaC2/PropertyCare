namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.GetImageContent;

/// <summary>Streams one attachment back to a caller authorized to see its request.</summary>
public sealed class GetRequestImageContentQuery : IRequest<RequestImageContentDto>
{
    public int RequestId { get; set; }
    public int ImageId { get; set; }
}

namespace PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListImages;

public sealed class ListRequestImagesQuery : IRequest<IReadOnlyList<RequestImageDto>>
{
    public int RequestId { get; set; }
}

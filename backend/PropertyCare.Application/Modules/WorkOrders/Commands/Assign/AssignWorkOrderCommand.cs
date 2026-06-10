namespace PropertyCare.Application.Modules.WorkOrders.Commands.Assign;

/// <summary>Triage action: assigns a maintenance request to a technician by creating a work order.</summary>
public sealed class AssignWorkOrderCommand : IRequest<int>
{
    public int RequestId { get; set; }
    public int AssignedToUserId { get; set; }
    public string? Note { get; set; }
}

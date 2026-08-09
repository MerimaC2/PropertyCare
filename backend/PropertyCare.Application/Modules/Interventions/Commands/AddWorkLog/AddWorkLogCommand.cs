namespace PropertyCare.Application.Modules.Interventions.Commands.AddWorkLog;

public sealed class AddWorkLogCommand : IRequest<int>
{
    public int WorkOrderId { get; set; }
    public string Note { get; set; } = null!;
    public int MinutesSpent { get; set; }
}

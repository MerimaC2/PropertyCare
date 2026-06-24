namespace PropertyCare.Application.Modules.Dashboard;

public sealed class DashboardStatsDto
{
    public int TotalRequests { get; set; }
    public IReadOnlyList<CountByLabelDto> ByStatus { get; set; } = [];
    public IReadOnlyList<CountByLabelDto> ByPriority { get; set; } = [];
}

public sealed class CountByLabelDto
{
    public string Label { get; set; } = null!;
    public int Count { get; set; }
}

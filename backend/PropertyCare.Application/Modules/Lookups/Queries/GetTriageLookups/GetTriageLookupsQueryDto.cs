namespace PropertyCare.Application.Modules.Lookups.Queries.GetTriageLookups;

public sealed class GetTriageLookupsQueryDto
{
    public List<LookupItemDto> Technicians { get; init; } = [];
    public List<LookupItemDto> Statuses { get; init; } = [];
    public List<LookupItemDto> Priorities { get; init; } = [];
    public List<LookupItemDto> Buildings { get; init; } = [];
}

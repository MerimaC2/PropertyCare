namespace PropertyCare.Application.Modules.Lookups;

/// <summary>Generic id/name pair used to fill dropdowns on the frontend.</summary>
public sealed class LookupItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
}

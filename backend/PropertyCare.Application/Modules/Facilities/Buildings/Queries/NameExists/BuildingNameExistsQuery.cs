namespace PropertyCare.Application.Modules.Facilities.Buildings.Queries.NameExists;

/// <summary>Whether another building already uses the given name (for async uniqueness validation).</summary>
public sealed class BuildingNameExistsQuery : IRequest<bool>
{
    public string Name { get; set; } = null!;

    /// <summary>Building to ignore (the one being edited), so it does not clash with itself.</summary>
    public int? ExcludeId { get; set; }
}

namespace PropertyCare.Application.Modules.Interventions.Queries.ListMine;

/// <summary>The current technician's work orders with their logged work.</summary>
public sealed class ListMyInterventionsQuery : IRequest<IReadOnlyList<InterventionDto>> { }

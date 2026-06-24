namespace PropertyCare.Application.Modules.Facilities.AssetTypes.Queries.List;

/// <summary>All asset types for the tenant (management list and asset-form dropdown).</summary>
public sealed class ListAssetTypesQuery : IRequest<IReadOnlyList<AssetTypeDto>> { }

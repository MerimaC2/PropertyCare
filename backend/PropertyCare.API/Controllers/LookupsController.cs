using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Modules.Lookups;
using PropertyCare.Application.Modules.Lookups.Queries.GetRequestFormLookups;
using PropertyCare.Application.Modules.Lookups.Queries.GetTriageLookups;
using PropertyCare.Application.Modules.Lookups.Queries.ListBuildingTypes;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/lookups")]
public sealed class LookupsController(ISender sender) : ControllerBase
{
    /// <summary>Dropdown data for the create-request form (any authenticated user).</summary>
    [HttpGet("request-form")]
    public async Task<GetRequestFormLookupsQueryDto> GetRequestFormLookups(CancellationToken ct)
    {
        return await sender.Send(new GetRequestFormLookupsQuery(), ct);
    }

    /// <summary>Dropdown data for the admin triage screen.</summary>
    [HttpGet("triage")]
    [Authorize(Roles = UserRoleEntity.Names.Administrator)]
    public async Task<GetTriageLookupsQueryDto> GetTriageLookups(CancellationToken ct)
    {
        return await sender.Send(new GetTriageLookupsQuery(), ct);
    }

    /// <summary>Building types for the building create/edit form (admin).</summary>
    [HttpGet("building-types")]
    [Authorize(Roles = UserRoleEntity.Names.Administrator)]
    public async Task<IReadOnlyList<LookupItemDto>> GetBuildingTypes(CancellationToken ct)
    {
        return await sender.Send(new ListBuildingTypesQuery(), ct);
    }
}

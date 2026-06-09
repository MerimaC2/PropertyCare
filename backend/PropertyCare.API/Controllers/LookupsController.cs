using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Modules.Lookups.Queries.GetRequestFormLookups;

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
}

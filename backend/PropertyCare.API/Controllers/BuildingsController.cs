using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Common;
using PropertyCare.Application.Modules.Facilities.Buildings;
using PropertyCare.Application.Modules.Facilities.Buildings.Commands.Create;
using PropertyCare.Application.Modules.Facilities.Buildings.Commands.Delete;
using PropertyCare.Application.Modules.Facilities.Buildings.Commands.Update;
using PropertyCare.Application.Modules.Facilities.Buildings.Queries.List;
using PropertyCare.Application.Modules.Facilities.Buildings.Queries.Locations;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/buildings")]
[Authorize(Roles = UserRoleEntity.Names.Administrator)]
public sealed class BuildingsController(ISender sender) : ControllerBase
{
    /// <summary>Paged, searchable list of buildings.</summary>
    [HttpGet]
    public async Task<PageResult<BuildingDto>> List(
        [FromQuery] ListBuildingsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    /// <summary>All buildings that have coordinates, for the interactive map.</summary>
    [HttpGet("locations")]
    public async Task<IReadOnlyList<BuildingLocationDto>> Locations(CancellationToken ct)
    {
        return await sender.Send(new ListBuildingLocationsQuery(), ct);
    }

    /// <summary>Creates a building.</summary>
    [HttpPost]
    public async Task<ActionResult<int>> Create(
        [FromBody] CreateBuildingCommand command, CancellationToken ct)
    {
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Updates an existing building.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, [FromBody] UpdateBuildingCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
        return NoContent();
    }

    /// <summary>Soft-deletes a building (only when it has no units).</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteBuildingCommand { Id = id }, ct);
        return NoContent();
    }
}

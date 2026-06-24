using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Modules.Facilities.Units;
using PropertyCare.Application.Modules.Facilities.Units.Commands.Create;
using PropertyCare.Application.Modules.Facilities.Units.Commands.Delete;
using PropertyCare.Application.Modules.Facilities.Units.Commands.Update;
using PropertyCare.Application.Modules.Facilities.Units.Queries.ListByBuilding;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/units")]
[Authorize(Roles = UserRoleEntity.Names.Administrator)]
public sealed class UnitsController(ISender sender) : ControllerBase
{
    /// <summary>Units of the given building.</summary>
    [HttpGet]
    public async Task<IReadOnlyList<UnitDto>> ListByBuilding(
        [FromQuery] int buildingId, CancellationToken ct)
    {
        return await sender.Send(new ListUnitsByBuildingQuery { BuildingId = buildingId }, ct);
    }

    /// <summary>Creates a unit in a building.</summary>
    [HttpPost]
    public async Task<ActionResult<int>> Create(
        [FromBody] CreateUnitCommand command, CancellationToken ct)
    {
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Renames a unit.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, [FromBody] UpdateUnitCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
        return NoContent();
    }

    /// <summary>Soft-deletes a unit (only when it has no assets).</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteUnitCommand { Id = id }, ct);
        return NoContent();
    }
}

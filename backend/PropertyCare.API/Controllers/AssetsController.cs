using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Modules.Facilities.Assets;
using PropertyCare.Application.Modules.Facilities.Assets.Commands.Create;
using PropertyCare.Application.Modules.Facilities.Assets.Commands.Delete;
using PropertyCare.Application.Modules.Facilities.Assets.Commands.Update;
using PropertyCare.Application.Modules.Facilities.Assets.Queries.ListByUnit;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/assets")]
[Authorize(Roles = UserRoleEntity.Names.Administrator)]
public sealed class AssetsController(ISender sender) : ControllerBase
{
    /// <summary>Assets of the given unit.</summary>
    [HttpGet]
    public async Task<IReadOnlyList<AssetDto>> ListByUnit(
        [FromQuery] int unitId, CancellationToken ct)
    {
        return await sender.Send(new ListAssetsByUnitQuery { UnitId = unitId }, ct);
    }

    /// <summary>Creates an asset in a unit.</summary>
    [HttpPost]
    public async Task<ActionResult<int>> Create(
        [FromBody] CreateAssetCommand command, CancellationToken ct)
    {
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Updates an asset.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, [FromBody] UpdateAssetCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
        return NoContent();
    }

    /// <summary>Soft-deletes an asset.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteAssetCommand { Id = id }, ct);
        return NoContent();
    }
}

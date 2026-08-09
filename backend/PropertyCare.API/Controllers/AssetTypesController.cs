using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Modules.Facilities.AssetTypes;
using PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Create;
using PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Delete;
using PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Update;
using PropertyCare.Application.Modules.Facilities.AssetTypes.Queries.List;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/asset-types")]
[Authorize(Roles = UserRoleEntity.Names.Administrator)]
public sealed class AssetTypesController(ISender sender) : ControllerBase
{
    /// <summary>All asset types.</summary>
    [HttpGet]
    public async Task<IReadOnlyList<AssetTypeDto>> List(CancellationToken ct)
    {
        return await sender.Send(new ListAssetTypesQuery(), ct);
    }

    /// <summary>Creates an asset type.</summary>
    [HttpPost]
    public async Task<ActionResult<int>> Create(
        [FromBody] CreateAssetTypeCommand command, CancellationToken ct)
    {
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Updates an asset type.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, [FromBody] UpdateAssetTypeCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
        return NoContent();
    }

    /// <summary>Soft-deletes an asset type (only when no asset uses it).</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteAssetTypeCommand { Id = id }, ct);
        return NoContent();
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Modules.Interventions;
using PropertyCare.Application.Modules.Interventions.Commands.AddWorkLog;
using PropertyCare.Application.Modules.Interventions.Queries.ListMine;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/interventions")]
[Authorize(Roles = UserRoleEntity.Names.Technician)]
public sealed class InterventionsController(ISender sender) : ControllerBase
{
    /// <summary>The current technician's work orders with their logged work.</summary>
    [HttpGet]
    public async Task<IReadOnlyList<InterventionDto>> ListMine(CancellationToken ct)
    {
        return await sender.Send(new ListMyInterventionsQuery(), ct);
    }

    /// <summary>Logs work (note + minutes spent) on one of the technician's work orders.</summary>
    [HttpPost("{workOrderId:int}/logs")]
    public async Task<ActionResult<int>> AddLog(
        int workOrderId, [FromBody] AddWorkLogCommand command, CancellationToken ct)
    {
        command.WorkOrderId = workOrderId;
        return Ok(await sender.Send(command, ct));
    }
}

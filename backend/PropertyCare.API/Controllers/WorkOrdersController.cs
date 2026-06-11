using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Modules.WorkOrders.Commands.Assign;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/work-orders")]
public sealed class WorkOrdersController(ISender sender) : ControllerBase
{
    /// <summary>Triage action: assigns a request to a technician by creating a work order.</summary>
    [HttpPost("assign")]
    [Authorize(Roles = UserRoleEntity.Names.Administrator)]
    public async Task<ActionResult<int>> Assign(
        [FromBody] AssignWorkOrderCommand command,
        CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return Ok(id);
    }
}

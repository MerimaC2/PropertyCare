using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.API.Models;
using PropertyCare.Application.Common;
using PropertyCare.Application.Modules.MaintenanceRequests;
using PropertyCare.Application.Modules.MaintenanceRequests.Commands.Create;
using PropertyCare.Application.Modules.MaintenanceRequests.Commands.UploadImage;
using PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListForTriage;
using PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListImages;
using PropertyCare.Application.Modules.MaintenanceRequests.Queries.ListMy;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/maintenance-requests")]
public sealed class MaintenanceRequestsController(ISender sender) : ControllerBase
{
    /// <summary>Creates a new fault report (reporter only).</summary>
    [HttpPost]
    [Authorize(Roles = UserRoleEntity.Names.Reporter)]
    public async Task<ActionResult<int>> Create(
        [FromBody] CreateMaintenanceRequestCommand command,
        CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return Ok(id);
    }

    /// <summary>Paged list of the current reporter's own requests with filters.</summary>
    [HttpGet("my")]
    [Authorize(Roles = UserRoleEntity.Names.Reporter)]
    public async Task<PageResult<ListMyMaintenanceRequestsQueryDto>> ListMy(
        [FromQuery] ListMyMaintenanceRequestsQuery query,
        CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    /// <summary>Paged, filterable and sortable list of all requests for admin triage.</summary>
    [HttpGet("triage")]
    [Authorize(Roles = UserRoleEntity.Names.Administrator)]
    public async Task<PageResult<ListTriageRequestsQueryDto>> ListForTriage(
        [FromQuery] ListTriageRequestsQuery query,
        CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    /// <summary>Attaches a photo to one of the reporter's own requests (multipart upload).</summary>
    [HttpPost("{id:int}/images")]
    [Authorize(Roles = UserRoleEntity.Names.Reporter)]
    public async Task<ActionResult<RequestImageDto>> UploadImage(
        int id,
        IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ErrorDto { Code = "validation.error", Message = "A file is required." });

        await using var content = file.OpenReadStream();
        var dto = await sender.Send(new UploadRequestImageCommand
        {
            RequestId = id,
            FileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            Content = content
        }, ct);

        return Ok(dto);
    }

    /// <summary>Lists the photos attached to one of the reporter's own requests.</summary>
    [HttpGet("{id:int}/images")]
    [Authorize(Roles = UserRoleEntity.Names.Reporter)]
    public async Task<ActionResult<IReadOnlyList<RequestImageDto>>> ListImages(
        int id,
        CancellationToken ct)
    {
        return Ok(await sender.Send(new ListRequestImagesQuery { RequestId = id }, ct));
    }
}

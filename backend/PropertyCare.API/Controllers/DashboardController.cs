using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Modules.Dashboard;
using PropertyCare.Application.Modules.Dashboard.Queries.GetStats;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = UserRoleEntity.Names.Administrator)]
public sealed class DashboardController(ISender sender) : ControllerBase
{
    /// <summary>Aggregated request statistics for the dashboard charts.</summary>
    [HttpGet("stats")]
    public async Task<DashboardStatsDto> Stats(CancellationToken ct)
    {
        return await sender.Send(new GetDashboardStatsQuery(), ct);
    }
}

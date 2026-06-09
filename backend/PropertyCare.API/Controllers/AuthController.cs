using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyCare.Application.Modules.Auth.Commands.Login;
using PropertyCare.Application.Modules.Auth.Commands.Logout;
using PropertyCare.Application.Modules.Auth.Commands.Refresh;

namespace PropertyCare.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginCommandDto>> Login(
        [FromBody] LoginCommand command,
        CancellationToken ct)
    {
        return Ok(await sender.Send(command, ct));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginCommandDto>> Refresh(
        [FromBody] RefreshTokenCommand command,
        CancellationToken ct)
    {
        return Ok(await sender.Send(command, ct));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task Logout(
        [FromBody] LogoutCommand command,
        CancellationToken ct)
    {
        await sender.Send(command, ct);
    }
}

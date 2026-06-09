using System.Security.Claims;
using PropertyCare.Application.Abstractions;
using PropertyCare.Infrastructure.Auth;

namespace PropertyCare.API.Services;

/// <summary>Reads the current user from the JWT claims of the active HTTP request.</summary>
public sealed class AppCurrentUser : IAppCurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppCurrentUser(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public int? UserId =>
        int.TryParse(Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public int? TenantId =>
        int.TryParse(Principal?.FindFirstValue(JwtTokenService.TenantIdClaim), out var id) ? id : null;

    public string? Email => Principal?.FindFirstValue(ClaimTypes.Email);

    public string? Role => Principal?.FindFirstValue(ClaimTypes.Role);
}

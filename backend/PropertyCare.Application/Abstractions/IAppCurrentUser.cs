namespace PropertyCare.Application.Abstractions;

/// <summary>
/// Information about the currently authenticated user, read from the JWT claims.
/// </summary>
public interface IAppCurrentUser
{
    bool IsAuthenticated { get; }
    int? UserId { get; }
    int? TenantId { get; }
    string? Email { get; }
    string? Role { get; }
}

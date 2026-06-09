using PropertyCare.Application.Abstractions;

namespace PropertyCare.Tests.Common;

public sealed class FakeCurrentUser : IAppCurrentUser
{
    public bool IsAuthenticated => UserId.HasValue;
    public int? UserId { get; init; }
    public int? TenantId { get; init; } = 1;
    public string? Email { get; init; }
    public string? Role { get; init; }
}

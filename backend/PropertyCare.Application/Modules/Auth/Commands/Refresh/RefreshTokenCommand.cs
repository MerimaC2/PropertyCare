using PropertyCare.Application.Modules.Auth.Commands.Login;

namespace PropertyCare.Application.Modules.Auth.Commands.Refresh;

public sealed class RefreshTokenCommand : IRequest<LoginCommandDto>
{
    public string RefreshToken { get; init; } = null!;
    public string? Fingerprint { get; init; }
}

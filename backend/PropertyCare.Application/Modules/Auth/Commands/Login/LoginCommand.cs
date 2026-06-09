namespace PropertyCare.Application.Modules.Auth.Commands.Login;

public sealed class LoginCommand : IRequest<LoginCommandDto>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;

    /// <summary>Optional device fingerprint for device-bound refresh tokens.</summary>
    public string? Fingerprint { get; init; }
}

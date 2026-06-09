namespace PropertyCare.Application.Modules.Auth.Commands.Logout;

public sealed class LogoutCommand : IRequest
{
    public string RefreshToken { get; init; } = null!;
}

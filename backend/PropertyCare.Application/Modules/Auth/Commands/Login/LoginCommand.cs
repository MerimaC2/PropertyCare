namespace PropertyCare.Application.Modules.Auth.Commands.Login;

public sealed class LoginCommand : IRequest<LoginCommandDto>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}

namespace PropertyCare.Application.Modules.Auth.Commands.Login;

public sealed class LoginCommandDto
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public string AccessTokenExpiresAtUtc { get; init; } = null!;

    public int UserId { get; init; }
    public string Email { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public string Role { get; init; } = null!;
}

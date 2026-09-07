namespace PropertyCare.Application.Common.Exceptions;

/// <summary>
/// Thrown when credentials are missing or wrong. Mapped to HTTP 401 with a message that is the same
/// for every reason, so a caller cannot tell an unknown account from a wrong password.
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}

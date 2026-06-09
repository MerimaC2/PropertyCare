namespace PropertyCare.Application.Common.Exceptions;

/// <summary>Thrown when the current user is not allowed to perform the action. Mapped to HTTP 403.</summary>
public sealed class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}

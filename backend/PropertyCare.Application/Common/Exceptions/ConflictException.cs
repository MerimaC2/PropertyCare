namespace PropertyCare.Application.Common.Exceptions;

/// <summary>Thrown when an operation conflicts with the current state. Mapped to HTTP 409.</summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}

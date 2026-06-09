namespace PropertyCare.API.Models;

/// <summary>Uniform error payload returned by the API for all failures.</summary>
public sealed class ErrorDto
{
    public string Code { get; init; } = null!;
    public string Message { get; init; } = null!;
    public string? Details { get; init; }
    public List<FieldErrorDto>? Errors { get; init; }
}

public sealed class FieldErrorDto
{
    public string Field { get; init; } = null!;
    public string Message { get; init; } = null!;
}

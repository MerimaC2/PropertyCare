using Microsoft.AspNetCore.Diagnostics;
using PropertyCare.API.Models;
using PropertyCare.Application.Common.Exceptions;

namespace PropertyCare.API.Middleware;

/// <summary>
/// Maps application exceptions to HTTP status codes with a uniform <see cref="ErrorDto"/> body.
/// </summary>
public sealed class AppExceptionHandler : IExceptionHandler
{
    private readonly ILogger<AppExceptionHandler> _logger;

    public AppExceptionHandler(ILogger<AppExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
    {
        var (statusCode, error) = exception switch
        {
            FluentValidation.ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new ErrorDto
                {
                    Code = "validation.error",
                    Message = "Validation failed.",
                    Errors = validationEx.Errors
                        .Select(e => new FieldErrorDto { Field = e.PropertyName, Message = e.ErrorMessage })
                        .ToList()
                }),
            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new ErrorDto { Code = "not.found", Message = notFoundEx.Message }),
            ConflictException conflictEx => (
                StatusCodes.Status409Conflict,
                new ErrorDto { Code = "conflict", Message = conflictEx.Message }),
            ForbiddenException forbiddenEx => (
                StatusCodes.Status403Forbidden,
                new ErrorDto { Code = "forbidden", Message = forbiddenEx.Message }),
            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorDto { Code = "server.error", Message = "An unexpected error occurred." })
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(error, ct);

        return true;
    }
}

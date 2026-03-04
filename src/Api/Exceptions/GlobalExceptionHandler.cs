using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Middleware;
using Template.Application.Exceptions;

namespace Template.Api.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var correlationId = httpContext.Items[CorrelationIdMiddleware.ItemKey]?.ToString()
            ?? httpContext.TraceIdentifier;
        var traceId = Activity.Current?.TraceId.ToHexString() ?? httpContext.TraceIdentifier;

        var (statusCode, title, detail) = MapException(exception);

        logger.LogError(
            exception,
            "Unhandled exception for request {Method} {Path}. CorrelationId: {CorrelationId}. TraceId: {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            correlationId,
            traceId);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["correlationId"] = correlationId;
        problemDetails.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title, string? Detail) MapException(Exception exception)
    {
        return exception switch
        {
            ExternalAuthValidationException => (StatusCodes.Status401Unauthorized, "Authentication failed.", exception.Message),
            ValidationException => (StatusCodes.Status400BadRequest, "Validation failed.", exception.Message),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found.", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "Request conflict.", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null)
        };
    }
}

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreAPI.Infrastructure;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
            ArgumentException         => (StatusCodes.Status400BadRequest, "Bad Request"),
            KeyNotFoundException      => (StatusCodes.Status404NotFound, "Not Found"),
            _                         => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        logger.LogError(
            exception,
            "Unhandled exception: {ExceptionType} — {Message} | Path: {Path} | StatusCode: {StatusCode}",
            exception.GetType().Name,
            exception.Message,
            context.Request.Path,
            statusCode);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}

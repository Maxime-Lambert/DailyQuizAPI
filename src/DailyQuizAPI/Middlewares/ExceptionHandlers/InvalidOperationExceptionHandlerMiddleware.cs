using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DailyQuizAPI.Middlewares.ExceptionHandlers;

public sealed partial class InvalidOperationExceptionHandlerMiddleware(ILogger<InvalidOperationExceptionHandlerMiddleware> logger) : IExceptionHandler
{
    private readonly ILogger<InvalidOperationExceptionHandlerMiddleware> _logger = logger;

    private const string BAD_REQUEST_ERROR_TITLE = "Bad Request";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not InvalidOperationException)
            return false;

        _logger.LogInvalidOperationException(exception.Message, exception);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = BAD_REQUEST_ERROR_TITLE,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

        return true;
    }
}


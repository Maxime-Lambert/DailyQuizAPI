using DailyQuizAPI.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DailyQuizAPI.Middlewares.ExceptionHandlers;

public sealed partial class NotFoundExceptionHandlerMiddleware(ILogger<NotFoundExceptionHandlerMiddleware> logger) : IExceptionHandler
{
    private readonly ILogger<NotFoundExceptionHandlerMiddleware> _logger = logger;

    private const string NOT_FOUND_ERROR_TITLE = "Ressource Not Found";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException)
            return false;

        _logger.LogNotFoundException(exception.Message, exception);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = NOT_FOUND_ERROR_TITLE,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

        return true;
    }
}


using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace DailyQuizAPI.Middlewares.ExceptionHandlers;

public sealed partial class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger) : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger = logger;

    private const string INTERNAL_SERVER_ERROR_TITLE = "Internal Server Error";
    private const string GENERAL_EXCEPTION_DETAIL = "An unexpected error has occured";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogUnhandledException(exception.Message, exception);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = INTERNAL_SERVER_ERROR_TITLE,
            Detail = GENERAL_EXCEPTION_DETAIL,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

        return true;
    }
}


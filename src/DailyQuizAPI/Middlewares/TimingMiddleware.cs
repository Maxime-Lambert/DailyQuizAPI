using System.Diagnostics;

namespace DailyQuizAPI.Middlewares;

public sealed class RequestTimingMiddleware(ILogger<RequestTimingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<RequestTimingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var sw = Stopwatch.StartNew();
        await next(context).ConfigureAwait(false);
        sw.Stop();

        _logger.LogRequestTiming(context.Request.Path, sw.ElapsedMilliseconds);
    }
}

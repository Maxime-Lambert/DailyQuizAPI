using DailyQuizAPI.Middlewares.ExceptionHandlers;

namespace DailyQuizAPI.Middlewares;

public static class CustomMiddlewaresSetup
{
    public static IServiceCollection AddCustomMiddlewares(this IServiceCollection services)
    {
        services.AddExceptionHandler<InvalidOperationExceptionHandlerMiddleware>();
        services.AddExceptionHandler<NotFoundExceptionHandlerMiddleware>();
        services.AddExceptionHandler<ExceptionHandlerMiddleware>();
        services.AddTransient<RequestTimingMiddleware>();
        return services;
    }

    public static WebApplication UseCustomMiddlewares(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseMiddleware<RequestTimingMiddleware>();
        return app;
    }
}

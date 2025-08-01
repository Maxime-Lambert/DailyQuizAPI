namespace DailyQuizAPI.Middlewares;

public static class CorsSetup
{
    private const string CORS_POLICY_NAME = "AllowFrontend";

    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {

        services.AddCors(options =>
        {
            options.AddPolicy(CORS_POLICY_NAME, policy =>
            {
                policy.WithOrigins(FrontEndOrigins.SUMOT)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }

    public static WebApplication UseCustomCors(this WebApplication app)
    {
        app.UseCors(CORS_POLICY_NAME);
        return app;
    }
}

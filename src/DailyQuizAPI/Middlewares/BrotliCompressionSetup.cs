using Microsoft.AspNetCore.ResponseCompression;

namespace DailyQuizAPI.Middlewares;

public static class BrotliCompressionSetup
{
    public static IServiceCollection AddBrotliCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        return services;
    }

    public static WebApplication UseBrotliCompression(this WebApplication app)
    {
        app.UseResponseCompression();
        return app;
    }
}
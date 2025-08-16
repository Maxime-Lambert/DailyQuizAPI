using System.Security.Claims;
using System.Threading.RateLimiting;

namespace DailyQuizAPI.Middlewares;

public static class RateLimiterSetup
{
    private const string POLICY_NAME = "ByAuth";
    private const string ANONYMOUS_USER_KEY = "anon";
    private const string UNKNOWN_USER_KEY = "unknown";
    private const int AUTHENTICATED_USER_PERMIT_LIMIT = 600;
    private const int ANONYMOUS_USER_PERMIT_LIMIT = 60;
    private const int RATE_LIMIT_WINDOW_MINUTES = 1;
    private const int RATE_LIMIT_QUEUE_LIMIT = 2;

    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddPolicy(POLICY_NAME, context =>
            {
                var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
                var key = isAuthenticated
                    ? context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ANONYMOUS_USER_KEY
                    : context.Connection.RemoteIpAddress?.ToString() ?? UNKNOWN_USER_KEY;

                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = isAuthenticated ? AUTHENTICATED_USER_PERMIT_LIMIT : ANONYMOUS_USER_PERMIT_LIMIT,
                    Window = TimeSpan.FromMinutes(RATE_LIMIT_WINDOW_MINUTES),
                    QueueLimit = RATE_LIMIT_QUEUE_LIMIT,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
        });

        return services;
    }
}

using DailyQuizAPI.Middlewares.Authentication;

namespace DailyQuizAPI.Middlewares;

public static class AuthorizationSetup
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(SecurityPolicies.PLAYER, policy =>
            {
                policy.RequireAuthenticatedUser().AddAuthenticationSchemes(AuthSchemes.JWT);
            })
            .AddPolicy(SecurityPolicies.SYSTEM, policy =>
            {
                policy.RequireAuthenticatedUser().AddAuthenticationSchemes(AuthSchemes.APIKEY);
            });

        return services;
    }
}

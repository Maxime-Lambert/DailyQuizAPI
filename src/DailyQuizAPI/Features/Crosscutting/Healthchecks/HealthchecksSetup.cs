using DailyQuizAPI.Persistence.Options;
using Microsoft.Extensions.Options;

namespace DailyQuizAPI.Features.Crosscutting.Healthchecks;

public static class HealthchecksSetup
{
    public static IServiceCollection AddCustomHealthchecks(this IServiceCollection services)
    {
        services.AddHealthChecks().AddNpgSql(
            sp => sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString!
        );

        return services;
    }
}

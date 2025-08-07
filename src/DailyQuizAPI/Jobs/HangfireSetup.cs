using Hangfire;
using Hangfire.PostgreSql;

namespace DailyQuizAPI.Jobs;

public static class HangfireSetup
{
    private const string DAILY_SUMOT_JOB_NAME = "daily-sumot";
    private const string DAILY_INACTIVE_JOB_NAME = "daily-inactive-check";
    private const int DAILY_SUMOT_JOB_HOUR = 0;
    private const int DAILY_INACTIVE_JOB_HOUR = 1;

    public static IServiceCollection AddCustomHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(configuration.GetConnectionString("Database")!);
            });
        });
        services.AddHangfireServer();

        return services;
    }
    public static WebApplication RegisterRecurringJobs(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        TimeZoneInfo parisTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
        var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        recurringJobs.AddOrUpdate<ChoseSumotOfTheDay>(
            DAILY_SUMOT_JOB_NAME,
            job => job.RunAsync(CancellationToken.None),
            Cron.Daily(DAILY_SUMOT_JOB_HOUR),
            new RecurringJobOptions
            {
                TimeZone = parisTimeZone
            }
        );

        recurringJobs.AddOrUpdate<DeleteInactiveUsers>(
            DAILY_INACTIVE_JOB_NAME,
            job => job.RunAsync(CancellationToken.None),
            Cron.Daily(DAILY_INACTIVE_JOB_HOUR),
            new RecurringJobOptions
            {
                TimeZone = parisTimeZone
            }
        );

        return app;
    }
}


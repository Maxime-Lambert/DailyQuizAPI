using DailyQuizAPI.Features.Crosscutting.Users;
using DailyQuizAPI.Persistence.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DailyQuizAPI.Persistence;

public static class PersistenceSetup
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.ConfigureOptions<DatabaseOptionsSetup>();

        services.AddDbContext<QuizContext>((serviceProvider, dbContextOptionsBuilder) =>
        {
            var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;

            dbContextOptionsBuilder.UseNpgsql(databaseOptions.ConnectionString, npgsqlOptionsAction =>
            {
                npgsqlOptionsAction.CommandTimeout(databaseOptions.CommandTimeout);
                npgsqlOptionsAction.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
            });
            dbContextOptionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
            dbContextOptionsBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
        });

        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<QuizContext>()
            .AddTokenProvider<RollbackTokenProvider<User>>("Rollback")
            .AddDefaultTokenProviders();

        return services;
    }
}

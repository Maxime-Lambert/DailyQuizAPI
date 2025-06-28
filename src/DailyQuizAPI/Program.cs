using DailyQuizAPI.AppSettings.CreateAppSetting;
using DailyQuizAPI.Persistence;
using DailyQuizAPI.Sumots.ExtractSumots;
using DailyQuizAPI.Sumots.GetSumots;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOptions<DatabaseOptionsSetup>();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<QuizContext>((serviceProvider, dbContextOptionsBuilder) =>
{
    var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;

    dbContextOptionsBuilder.UseNpgsql(databaseOptions.ConnectionString, npgsqlOptionsAction =>
    {
        npgsqlOptionsAction.CommandTimeout(databaseOptions.CommandTimeout);
        npgsqlOptionsAction.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
    });
    dbContextOptionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
    dbContextOptionsBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
    dbContextOptionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddHealthChecks();

builder.Services.AddScoped<GetSumotsQueryHandler>();
builder.Services.AddScoped<CreateAppSettingCommandHandler>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapGetSumotsEndpoint();
app.MapPostAppSettingEndpoint();
app.MapExtractSumotsEndpoint();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
})
.WithName("Health")
.WithOpenApi()
.WithTags("System");

await app.ApplyMigrationsAsync().ConfigureAwait(false);

await app.RunAsync().ConfigureAwait(false);

public partial class Program
{
    private Program() { }
}


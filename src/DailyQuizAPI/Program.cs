using DailyQuizAPI.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});


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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

/*app.MapGet("/FiveLettersFrenchWords", async (QuizContext quizContext) =>
{
    using var http = new HttpClient();
    var sumots = File.ReadAllLines("D:\\Development\\source\\Maxime-Lambert\\DailyQuiz\\src\\DailyQuizAPI\\ods6.txt")
                     .Where(w => w.Length == 5)
                     .Distinct()
                     .Select(w => new Sumot { Word = w.Trim().ToLowerInvariant(), Day = null });
    quizContext.Sumots.AddRange(sumots);
    await quizContext.SaveChangesAsync();
})
.WithName("FiveLettersFrenchWords")
.WithOpenApi();*/

app.MapGet("/Sumots", async (QuizContext quizContext) =>
{
    var sumots = await quizContext.Sumots.ToListAsync();
    return Results.Ok(sumots);
})
.WithName("GetSumots")
.WithOpenApi();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
})
.WithName("Health")
.WithOpenApi()
.WithTags("System");

app.Run();

using DailyQuizAPI.Features;
using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Features.Crosscutting.Healthchecks;
using DailyQuizAPI.Features.SumotApp.Ranking;
using DailyQuizAPI.Jobs;
using DailyQuizAPI.Logger;
using DailyQuizAPI.Mail;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Middlewares.Authentication;
using DailyQuizAPI.OpenApi;
using DailyQuizAPI.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.UseSerilog();

builder.Services
    .AddCustomCors()
    .AddCustomHealthchecks()
    .AddCommandHandlers()
    .AddCustomHangfire(builder.Configuration)
    .AddCustomAuthentication(builder.Configuration)
    .AddAuthorizationPolicies()
    .AddCustomRateLimiter()
    .AddCustomSwagger()
    .AddPersistence()
    .AddScoped<IRankingService, RankingService>()
    .AddSingleton<ICacheService, MemoryCacheService>()
    .AddSmtpEmail(builder.Configuration)
    .AddProblemDetails()
    .AddMemoryCache()
    .AddCustomMiddlewares();

var app = builder.Build();

app.UseCustomCors();
app.UseHttpsRedirection();
app.UseCustomMiddlewares();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.RegisterRecurringJobs()
    .UseSwaggerDark()
    .MapEndpoints()
    .UseSerilogRequestLogging();

await app.ApplyMigrationsAsync().ConfigureAwait(false);

await app.RunAsync().ConfigureAwait(false);

public partial class Program
{
    private Program() { }
}


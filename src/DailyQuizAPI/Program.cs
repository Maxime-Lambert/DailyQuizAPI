using DailyQuizAPI.AppSettings.Create;
using DailyQuizAPI.Persistence;
using DailyQuizAPI.Persistence.Options;
using DailyQuizAPI.Sumots.Extract;
using DailyQuizAPI.Sumots.GetAll;
using DailyQuizAPI.Users;
using DailyQuizAPI.Users.Create;
using DailyQuizAPI.Users.Login;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOptions<DatabaseOptionsSetup>();
builder.Services.ConfigureOptions<AuthenticationOptionsSetup>();

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
builder.Services.AddScoped<ExtractSumotsCommandHandler>();
builder.Services.AddScoped<CreateAppSettingCommandHandler>();
builder.Services.AddScoped<CreateUserCommandHandler>();
builder.Services.AddScoped<LoginCommandHandler>();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<QuizContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
    };
});

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapGetSumotsEndpoint();
app.MapPostAppSettingEndpoint();
app.MapExtractSumotsEndpoint();
app.MapCreateUserEndpoint();
app.MapLoginEndpoint();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
})
.WithName("Health")
.WithOpenApi()
.WithTags("System");

await app.ApplyMigrationsAsync().ConfigureAwait(false);

Console.WriteLine(Convert.ToBase64String(RandomNumberGenerator.GetBytes(128)));

await app.RunAsync().ConfigureAwait(false);

public partial class Program
{
    private Program() { }
}


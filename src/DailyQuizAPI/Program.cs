using DailyQuizAPI.Features.AppSettings.Create;
using DailyQuizAPI.Features.Healthchecks.GetAll;
using DailyQuizAPI.Features.SumotHistories.Add;
using DailyQuizAPI.Features.SumotHistories.GetAll;
using DailyQuizAPI.Features.Sumots.Extract;
using DailyQuizAPI.Features.Sumots.GetAll;
using DailyQuizAPI.Features.Users;
using DailyQuizAPI.Features.Users.Create;
using DailyQuizAPI.Features.Users.Login;
using DailyQuizAPI.Features.Users.Refresh;
using DailyQuizAPI.Persistence;
using DailyQuizAPI.Persistence.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT au format **Bearer &lt;token&gt;**"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.ConfigureOptions<DatabaseOptionsSetup>();
builder.Services.ConfigureOptions<AuthenticationOptionsSetup>();

builder.Services.AddProblemDetails();

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

builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

RegisterCommandHandlers(builder);

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

MapApiEndpoints(app);

await app.ApplyMigrationsAsync().ConfigureAwait(false);

Console.WriteLine(Convert.ToBase64String(RandomNumberGenerator.GetBytes(128)));

await app.RunAsync().ConfigureAwait(false);


void RegisterCommandHandlers(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<CreateAppSettingCommandHandler>();
    builder.Services.AddScoped<CreateUserCommandHandler>();
    builder.Services.AddScoped<AddSumotHistoryCommandHandler>();
    builder.Services.AddScoped<ExtractSumotsCommandHandler>();
    builder.Services.AddScoped<LoginCommandHandler>();
    builder.Services.AddScoped<RefreshCommandHandler>();
    builder.Services.AddScoped<GetSumotsQueryHandler>();
    builder.Services.AddScoped<GetSumotHistoriesQueryHandler>();
}

void MapApiEndpoints(WebApplication app)
{
    app.MapGetSumotsEndpoint();
    app.MapPostAppSettingEndpoint();
    app.MapExtractSumotsEndpoint();
    app.MapCreateUserEndpoint();
    app.MapLoginEndpoint();
    app.MapGetHealthchecks();
    app.MapRefreshEndpoint();
    app.MapGetSumotHistoriesEndpoint();
    app.MapAddSumotHistoryEndpoint();
}

public partial class Program
{
    private Program() { }
}


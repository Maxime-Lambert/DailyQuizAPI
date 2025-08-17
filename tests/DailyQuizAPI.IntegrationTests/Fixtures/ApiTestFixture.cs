using DailyQuizAPI.Features.Crosscutting.Users.Create;
using DailyQuizAPI.Features.Crosscutting.Users.Login;
using DailyQuizAPI.IntegrationTests.Containers;
using DailyQuizAPI.IntegrationTests.MockServices;
using DailyQuizAPI.Mail;
using DailyQuizAPI.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Testcontainers.PostgreSql;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Fixtures;

public class ApiTestFixture : IAsyncLifetime
{
    public HttpClient? Client { get; private set; }
    public PostgreSqlContainer? DbContainer { get; private set; }
    private WebApplicationFactory<Program>? _factory;
    public IConfiguration? Configuration { get; private set; }

    public async Task InitializeAsync()
    {
        try
        {
            DbContainer = PostgresTestContainer.Create();
            await DbContainer.StartAsync();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((_, config) =>
                    {
                        var settings = new Dictionary<string, string?>
                        {
                            ["ConnectionStrings:Database"] = DbContainer.GetConnectionString()
                        };
                        config.AddInMemoryCollection(settings!);
                        config.AddJsonFile("appsettings.Test.json", optional: true);
                    });
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(IEmailService));
                        if (descriptor is not null)
                        {
                            services.Remove(descriptor);
                        }
                        services.AddScoped<IEmailService, FakeEmailService>();
                    });
                });

            using var scope = _factory.Services.CreateScope();
            Configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            Client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            Client.Timeout = TimeSpan.FromSeconds(60);
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            Client?.Dispose();
            Client = null;
        }
        finally
        {
            _factory?.Dispose();
            _factory = null;

            if (DbContainer is not null)
            {
                await DbContainer.DisposeAsync();
                DbContainer = null;
            }
        }
    }

    public async Task<(string accessToken, string refreshToken)> RegisterAndLoginAsync(
        string userName, string email, string password)
    {
        if (Client is null) throw new InvalidOperationException("Fixture not initialized.");

        var createUserCommand = new CreateUserCommand(userName, email, password, 0);
        var response = await Client.PostAsJsonAsync("/users", createUserCommand);
        response.EnsureSuccessStatusCode();

        var loginPayload = new LoginCommand(userName, password);
        var loginResponse = await Client.PostAsJsonAsync("/users/login", loginPayload);
        loginResponse.EnsureSuccessStatusCode();

        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        return (body!.Token, body.RefreshToken);
    }

    public async Task<string> GetUserIdByUsernameAsync(string username)
    {
        if (_factory is null) throw new InvalidOperationException("Fixture not initialized.");
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<QuizContext>();
        var user = await context.Users.FirstAsync(u => u.UserName == username);
        return user.Id;
    }
}



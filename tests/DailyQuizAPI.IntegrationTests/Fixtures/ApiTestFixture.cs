using DailyQuizAPI.IntegrationTests.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Fixtures;

public class ApiTestFixture : IAsyncLifetime
{
    public HttpClient Client { get; private set; } = default!;
    public PostgreSqlContainer DbContainer { get; private set; } = default!;
    private WebApplicationFactory<Program> _factory = default!;

    public async Task InitializeAsync()
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
            });

        Client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await DbContainer.StopAsync();
        Client.Dispose();
        _factory.Dispose();
    }
}


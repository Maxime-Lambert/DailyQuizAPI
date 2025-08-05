using DailyQuizAPI.Features.Crosscutting.Users.Create;
using DailyQuizAPI.Features.Crosscutting.Users.Login;
using DailyQuizAPI.IntegrationTests.Containers;
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
    public HttpClient Client { get; private set; } = default!;
    public PostgreSqlContainer DbContainer { get; private set; } = default!;
    private WebApplicationFactory<Program> _factory = default!;
    public IConfiguration Configuration { get; private set; } = default!;

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
        var scope = _factory.Services.CreateScope();
        Configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        Client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await DbContainer.StopAsync();
        Client.Dispose();
        _factory.Dispose();
    }

    public async Task<(string accessToken, string refreshToken)> RegisterAndLoginAsync(string userName, string email, string password)
    {
        CreateUserCommand createUserCommand = new(userName, email, password, 0);

        var response = await Client.PostAsJsonAsync("/users", createUserCommand);
        response.EnsureSuccessStatusCode();

        LoginCommand loginPayload = new(userName, password);

        var loginResponse = await Client.PostAsJsonAsync("/users/login", loginPayload);
        loginResponse.EnsureSuccessStatusCode();

        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        return (body!.Token, body.RefreshToken);
    }

    public async Task<string> GetUserIdByUsernameAsync(string username)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<QuizContext>();
        var user = await context.Users.FirstAsync(u => u.UserName == username);
        return user.Id;
    }

}


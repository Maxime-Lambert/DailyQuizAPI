using DailyQuizAPI.Features.Users.Create;
using DailyQuizAPI.Features.Users.Login;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Users;

public class LoginTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task Login_ReturnsJwtToken_WhenCredentialsAreValid()
    {
        // Créer un utilisateur d’abord
        CreateUserCommand user = new("loginuser", "login@example.com", "LoginTest123!");
        var createResponse = await _client.PostAsJsonAsync("/users", user);
        createResponse.EnsureSuccessStatusCode();

        LoginCommand loginCommand = new(user.UserName, user.Password, "127.0.0.1");

        var loginResponse = await _client.PostAsJsonAsync("/users/login", loginCommand);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var token = await loginResponse.Content.ReadAsStringAsync();
        token.Should().NotBeNullOrWhiteSpace();
        token.Should().Contain(".");
    }
}

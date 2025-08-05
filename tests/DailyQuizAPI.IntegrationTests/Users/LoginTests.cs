using DailyQuizAPI.Features.Crosscutting.Users.Create;
using DailyQuizAPI.Features.Crosscutting.Users.Login;
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
        CreateUserCommand createUserCommand = new("loginuser", "login@example.com", "LoginTest123!", 0);
        var createUserResponse = await _client.PostAsJsonAsync("/users", createUserCommand);
        createUserResponse.EnsureSuccessStatusCode();

        LoginCommand loginCommand = new(createUserCommand.UserName, createUserCommand.Password);
        var loginResponse = await _client.PostAsJsonAsync("/users/login", loginCommand);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
}

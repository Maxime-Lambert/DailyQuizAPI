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
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task Login_ReturnsJwtToken_WhenCredentialsAreValid()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        CreateUserCommand createUserCommand = new($"user_{unique}", $"user_{unique}@example.com", "LoginTest123!", 0);
        var createUserResponse = await Client.PostAsJsonAsync("/users", createUserCommand);
        createUserResponse.EnsureSuccessStatusCode();

        LoginCommand loginCommand = new(createUserCommand.UserName, createUserCommand.Password);
        var loginResponse = await Client.PostAsJsonAsync("/users/login", loginCommand);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
}

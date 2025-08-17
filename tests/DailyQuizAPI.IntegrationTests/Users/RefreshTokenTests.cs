using DailyQuizAPI.Features.Crosscutting.Users.Login;
using DailyQuizAPI.Features.Crosscutting.Users.Refresh;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Users;

public sealed class RefreshTokenTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task Refresh_ReturnsNewAccessToken()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        var (token, refreshToken) = await fixture.RegisterAndLoginAsync($"user_{unique}", $"user_{unique}@example.com", "Test123!");

        RefreshCommand command = new(refreshToken);

        var response = await Client.PostAsJsonAsync("/users/refresh", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
        content.Should().NotBeNull();
        content!.Token.Should().NotBeNullOrWhiteSpace();
        content.RefreshToken.Should().NotBeNullOrWhiteSpace();
        content.Token.Should().NotBe(token);
    }
}


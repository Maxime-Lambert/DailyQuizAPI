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
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task Refresh_ReturnsNewAccessToken()
    {
        var (accessToken, refreshToken) = await fixture.RegisterAndLoginAsync("refreshtest", "refresh@example.com", "Test123!");

        RefreshCommand command = new(refreshToken);

        var response = await _client.PostAsJsonAsync("/users/refresh", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
        content.Should().NotBeNull();
        content!.Token.Should().NotBeNullOrWhiteSpace();
        content.RefreshToken.Should().NotBeNullOrWhiteSpace();
        content.Token.Should().NotBe(accessToken);
    }
}


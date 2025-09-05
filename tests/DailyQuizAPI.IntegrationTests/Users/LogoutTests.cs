using DailyQuizAPI.Features.Crosscutting.Users.Logout;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Users;

public sealed class LogoutTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task Logout_WithoutCookieAndHeader_ReturnsBadRequest()
    {
        // Arrange
        var unique = Guid.NewGuid().ToString("N")[..8];
        var (token, refreshToken) = await fixture.RegisterAndLoginAsync($"user_{unique}", $"user_{unique}@example.com", "Test123!");
        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        // Act
        LogoutCommand command = new(refreshToken);
        var response = await Client.PostAsJsonAsync("/users/logout", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Logout_WithSpaHeaderAndCookie_ReturnsNoContent()
    {
        // Arrange
        var unique = Guid.NewGuid().ToString("N")[..8];
        var (token, refreshToken) = await fixture.RegisterAndLoginAsync($"user_{unique}", $"user_{unique}@example.com", "Test123!");

        var request = new HttpRequestMessage(HttpMethod.Post, "/users/logout");
        request.Headers.Add("X-Client-Type", "SPA");

        Client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={refreshToken}");
        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}

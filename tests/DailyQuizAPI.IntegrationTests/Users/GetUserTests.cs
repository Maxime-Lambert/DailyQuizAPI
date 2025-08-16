using DailyQuizAPI.Features.Crosscutting.Users.GetOne;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Users;

public sealed class GetUserTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task UserGet_ReturnsOk()
    {
        await fixture.ResetDatabaseAsync();
        var unique = Guid.NewGuid().ToString("N")[..8];
        var (token, _) = await fixture.RegisterAndLoginAsync($"user_{unique}", $"user_{unique}@example.com", "Test123!");
        var userId = await fixture.GetUserIdByUsernameAsync($"user_{unique}");

        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var response = await Client.GetAsync($"/users/{userId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<GetUserResponse>();
        result.Should().NotBeNull();
    }
}


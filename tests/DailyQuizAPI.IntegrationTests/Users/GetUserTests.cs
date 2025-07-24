using DailyQuizAPI.Features.Crosscutting.Users.GetOne;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Users;

public sealed class GetUserTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task UserGet_ReturnsOk()
    {
        var (token, _) = await fixture.RegisterAndLoginAsync("updateuser", "update@example.com", "Test123!");
        var userId = await fixture.GetUserIdByUsernameAsync("updateuser");

        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var response = await _client.GetAsync($"/users/{userId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<GetUserResponse>();
        result.Should().NotBeNull();
    }
}


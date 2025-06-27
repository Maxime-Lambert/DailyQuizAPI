using DailyQuizAPI.IntegrationTests.Fixtures;
using DailyQuizAPI.Sumots.GetSumots;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Sumots;

public class GetSumotsTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task GetSumots_ReturnsOk_WithExpectedShape()
    {
        var response = await _client.GetAsync("/FiveLettersFrenchWords");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await _client.GetAsync("/sumots");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<List<GetSumotsResponse>>();
        content.Should().NotBeNull().And.HaveCountGreaterThan(0);
    }
}

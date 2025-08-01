using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Sumots;

public class GetSumotsTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task GetSumots_ReturnsOk()
    {
        var response = await _client.GetAsync("/sumots");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

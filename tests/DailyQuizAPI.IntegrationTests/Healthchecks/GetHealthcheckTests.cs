using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Healthchecks;

public class GetHealthcheckTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Healthy");
    }
}

using DailyQuizAPI.Features.Sumots.Extract;
using DailyQuizAPI.Features.Sumots.GetAll;
using DailyQuizAPI.IntegrationTests.Fixtures;
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
        ExtractSumotsCommand extractSumotsCommand = new(5);
        var response = await _client.PostAsJsonAsync("/sumots/extract", extractSumotsCommand);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await _client.GetAsync("/sumots");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<GetSumotsResponseList>();
        content.Should().NotBeNull();
    }
}

using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Sumots;

public class GetSumotsTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task GetSumots_ReturnsOk()
    {
        var response = await Client.GetAsync("/sumots");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

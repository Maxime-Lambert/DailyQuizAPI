using DailyQuizAPI.Features.SumotApp.SumotStats.AddFinish;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.SumotStats;

public class AddFinishTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task AddFinish_ReturnsOk()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        AddFinishCommand command = new(today, false);
        var response = await Client.PostAsJsonAsync("/sumotstats/addfinish", command);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
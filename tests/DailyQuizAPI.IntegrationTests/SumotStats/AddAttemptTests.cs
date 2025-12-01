using DailyQuizAPI.Features.SumotApp.SumotStats.AddAttempt;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.SumotStats;

public class AddAttemptTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task AddAttempt_ReturnsOk()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        AddAttemptCommand command = new(today, false);
        var response = await Client.PostAsJsonAsync("/sumotstats/addattempt", command);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
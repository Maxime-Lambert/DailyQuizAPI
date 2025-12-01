using DailyQuizAPI.Features.SumotApp.SumotStats.AddVisit;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.SumotStats;

public class AddVisitTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task AddVisit_ReturnsOk()
    {
        AddVisitCommand command = new(false);
        var response = await Client.PostAsJsonAsync("/sumotstats/addvisit", command);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
using DailyQuizAPI.Features.SumotApp.SumotHistories.Update;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.SumotHistories;

public sealed class UpdateSumotHistoriesTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task AddSumotHistories_ReturnsOk()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        var (token, _) = await fixture.RegisterAndLoginAsync($"user_{unique}", $"user_{unique}@example.com", "Test123!");
        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        UpdateSumotHistoriesCommand command = new(
            [new("rouge", ["verts", "bleue", "rouge"], true)], null
        );

        var response = await Client.PostAsJsonAsync("/sumothistories/updaterange", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}


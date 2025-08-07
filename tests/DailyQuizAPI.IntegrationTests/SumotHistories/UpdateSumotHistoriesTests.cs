using DailyQuizAPI.Features.SumotApp.SumotHistories.Update;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.SumotHistories;

public sealed class UpdateSumotHistoriesTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task AddSumotHistories_ReturnsOk()
    {
        var tokens = await fixture.RegisterAndLoginAsync("historyuser", "history@example.com", "Test123!");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tokens!.accessToken);
        UpdateSumotHistoriesCommand command = new(
            [new("rouge", ["verts", "bleue", "rouge"])]
        );

        var response = await _client.PostAsJsonAsync("/sumothistories/updaterange", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}


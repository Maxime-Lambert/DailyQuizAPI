using DailyQuizAPI.Features.SumotApp.SumotHistories.GetAll;
using DailyQuizAPI.Features.SumotApp.SumotHistories.Update;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.SumotHistories;

public sealed class GetSumotHistoriesTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task GetSumotHistories_ReturnsOk()
    {
        var tokens = await fixture.RegisterAndLoginAsync("gethistoryuser", "gethistory@example.com", "Test123!");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tokens.accessToken);

        UpdateSumotHistoriesCommand updateSumotHistoriesCommand = new(
            [new("bleue", ["bleue", "verte"])]
        );
        await _client.PostAsJsonAsync("/sumothistories/updaterange", updateSumotHistoriesCommand);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = new GetSumotHistoriesQuery(today, today);
        var uri = $"/sumothistories?MinDate={query.MinDate}&MaxDate={query.MaxDate}";

        var response = await _client.GetAsync(uri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<GetSumotHistoriesResponse>>();
        result.Should().NotBeNull();
    }
}


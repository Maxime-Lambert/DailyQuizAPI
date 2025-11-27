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
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task GetSumotHistories_ReturnsOk()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        var (token, _) = await fixture.RegisterAndLoginAsync($"user_{unique}", $"user_{unique}@example.com", "Test123!");
        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        UpdateSumotHistoriesCommand updateSumotHistoriesCommand = new(
            [new("bleue", ["bleue", "verte"], true)], null
        );
        await Client.PostAsJsonAsync("/sumothistories/updaterange", updateSumotHistoriesCommand);

        var parisTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, parisTz));
        var uri = $"/sumothistories?StartDate={today:yyyy-MM-dd}&EndDate={today:yyyy-MM-dd}";
        var response = await Client.GetAsync(uri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<GetSumotHistoriesResponse>>();
        result.Should().NotBeNull();
    }
}


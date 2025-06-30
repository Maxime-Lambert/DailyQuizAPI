using DailyQuizAPI.AppSettings.Create;
using DailyQuizAPI.IntegrationTests.Fixtures;
using DailyQuizAPI.Sumots.Extract;
using DailyQuizAPI.Sumots.GetAll;
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
        var response = await _client.PostAsJsonAsync("/extractSumots", extractSumotsCommand);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        CreateAppSettingCommand body = new("DatabaseVersion", "5");
        response = await _client.PostAsJsonAsync("/appsettings", body);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response = await _client.GetAsync("/sumots");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<GetSumotsResponseList>();
        content.Should().NotBeNull();
    }
}

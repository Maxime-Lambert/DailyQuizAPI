using DailyQuizAPI.AppSettings.Create;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.AppSettings;

public class AppSettingEndpointTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public AppSettingEndpointTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task CreateAppSetting_ReturnsCreated()
    {
        CreateAppSettingCommand body = new("DatabaseVersion", "5");

        var response = await _client.PostAsJsonAsync("/appsettings", body);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}


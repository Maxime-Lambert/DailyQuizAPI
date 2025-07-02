using DailyQuizAPI.Features.AppSettings.Create;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.AppSettings;

public class AppSettingEndpointTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task CreateAppSetting_ReturnsCreated()
    {
        CreateAppSettingCommand body = new("AppSetting Test", "15");

        var response = await _client.PostAsJsonAsync("/appsettings", body);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}


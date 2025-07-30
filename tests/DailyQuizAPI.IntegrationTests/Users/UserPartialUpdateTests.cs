using DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Users;

public sealed class UserPartialUpdateTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task UserPartialUpdate_UpdatesUserSuccessfully()
    {
        var (token, _) = await fixture.RegisterAndLoginAsync("updateuser", "update@example.com", "Test123!");
        var userId = await fixture.GetUserIdByUsernameAsync("updateuser");

        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        UserPartialUpdateCommand updateCommand = new(
            UserName: "newname",
            Email: "newemail@example.com",
            LastPassword: "Test123!",
            NewPassword: "NewPassword123!",
            KeyboardLayout: null,
            ColorblindMode: null,
            SmartKeyboardType: null
        );

        var response = await _client.PatchAsJsonAsync($"/users/{userId}", updateCommand);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}


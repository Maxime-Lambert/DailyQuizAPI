using DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Users;

public sealed class UserPartialUpdateTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task UserPartialUpdate_UpdatesUserSuccessfully()
    {
        await fixture.ResetDatabaseAsync();
        var unique = Guid.NewGuid().ToString("N")[..8];
        var (token, _) = await fixture.RegisterAndLoginAsync($"user_{unique}", $"user_{unique}@example.com", "Test123!");
        var userId = await fixture.GetUserIdByUsernameAsync($"user_{unique}");

        Client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var newUnique = Guid.NewGuid().ToString("N")[..8];
        PartialUpdateUserCommand updateCommand = new(
            Username: $"user_{newUnique}",
            Email: $"user_{newUnique}@example.com",
            LastPassword: "Test123!",
            NewPassword: "NewPassword123!",
            ColorblindMode: null,
            KeyboardLayout: null,
            SmartKeyboardType: null,
            0
        );

        var response = await Client.PatchAsJsonAsync($"/users/{userId}", updateCommand);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}


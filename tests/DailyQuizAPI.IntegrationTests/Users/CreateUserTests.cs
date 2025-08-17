using DailyQuizAPI.Features.Crosscutting.Users.Create;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Users;

public class CreateUserTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task CreateUser_Returns201_WhenUserIsValid()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        CreateUserCommand user = new($"user_{unique}", $"user_{unique}@example.com", "StrongPassword123!", 0);

        var response = await Client.PostAsJsonAsync("/users", user);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}

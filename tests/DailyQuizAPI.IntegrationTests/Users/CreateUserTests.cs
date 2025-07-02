using DailyQuizAPI.Features.Users.Create;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.Users;

public class CreateUserTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task CreateUser_Returns201_WhenUserIsValid()
    {
        CreateUserCommand user = new("testuser", "test@example.com", "StrongPassword123!");

        var response = await _client.PostAsJsonAsync("/users", user);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}

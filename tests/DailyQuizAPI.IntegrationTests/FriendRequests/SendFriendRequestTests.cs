using DailyQuizAPI.Features.Crosscutting.FriendRequests.Create;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.FriendRequests;

public sealed class SendFriendRequestTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task SendFriendRequest_ReturnsCreated()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        var (tokenSender, _) = await fixture.RegisterAndLoginAsync($"sender_{unique}", $"sender_{unique}@example.com", "Test123!");
        var (tokenReceiver, _) = await fixture.RegisterAndLoginAsync($"receiver_{unique}", $"receiver_{unique}@example.com", "Test123!");

        Client.DefaultRequestHeaders.Authorization = new("Bearer", tokenSender);

        CreateFriendRequestCommand command = new($"receiver_{unique}");

        var response = await Client.PostAsJsonAsync("/friendrequests", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}


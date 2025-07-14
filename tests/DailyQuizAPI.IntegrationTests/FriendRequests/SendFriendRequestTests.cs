using DailyQuizAPI.Features.Crosscutting.FriendRequests.Send;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.FriendRequests;

public sealed class SendFriendRequestTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task SendFriendRequest_ReturnsCreated()
    {
        var (tokenSender, _) = await fixture.RegisterAndLoginAsync("sender", "sender@example.com", "Test123!");
        var (tokenReceiver, _) = await fixture.RegisterAndLoginAsync("receiver", "receiver@example.com", "Test123!");

        _client.DefaultRequestHeaders.Authorization = new("Bearer", tokenSender);
        var receiverId = await fixture.GetUserIdByUsernameAsync("receiver");

        SendFriendRequestCommand command = new(receiverId);

        var response = await _client.PostAsJsonAsync("/friendrequests/send", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}


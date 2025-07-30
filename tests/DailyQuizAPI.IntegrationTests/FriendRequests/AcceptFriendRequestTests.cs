using DailyQuizAPI.Features.Crosscutting.FriendRequests.Accept;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.Send;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.FriendRequests;

public sealed class AcceptFriendRequestTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task AcceptFriendRequest_ReturnsCreated()
    {
        var (tokenSender, _) = await fixture.RegisterAndLoginAsync("sender2", "sender2@example.com", "Test123!");
        var (tokenReceiver, _) = await fixture.RegisterAndLoginAsync("receiver2", "receiver2@example.com", "Test123!");

        _client.DefaultRequestHeaders.Authorization = new("Bearer", tokenSender);
        await _client.PostAsJsonAsync("/friendrequests/send", new SendFriendRequestCommand("receiver2"));

        _client.DefaultRequestHeaders.Authorization = new("Bearer", tokenReceiver);
        var senderId = await fixture.GetUserIdByUsernameAsync("sender2");
        var response = await _client.PostAsJsonAsync("/friendrequests/accept", new AcceptFriendRequestCommand(senderId));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}


using DailyQuizAPI.Features.Crosscutting.FriendRequests.Accept;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.RemoveFriend;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.Send;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.FriendRequests;

public sealed class RemoveFriendTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task RemoveFriend_ReturnsOk()
    {
        var (tokenSender, _) = await fixture.RegisterAndLoginAsync("sender4", "sender4@example.com", "Test123!");
        var (tokenReceiver, _) = await fixture.RegisterAndLoginAsync("receiver4", "receiver4@example.com", "Test123!");

        var senderId = await fixture.GetUserIdByUsernameAsync("sender4");

        _client.DefaultRequestHeaders.Authorization = new("Bearer", tokenSender);
        await _client.PostAsJsonAsync("/friendrequests/send", new SendFriendRequestCommand("receiver4"));

        _client.DefaultRequestHeaders.Authorization = new("Bearer", tokenReceiver);
        await _client.PostAsJsonAsync("/friendrequests/accept", new AcceptFriendRequestCommand(senderId));

        var response = await _client.PostAsJsonAsync("/friendrequests/removefriend", new RemoveFriendCommand(senderId));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

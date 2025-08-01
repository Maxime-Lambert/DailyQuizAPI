using DailyQuizAPI.Features.Crosscutting.FriendRequests.Create;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.GetAll;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.FriendRequests;

public sealed class GetFriendRequestsTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task GetFriendRequests_ReturnsOk()
    {
        var (tokenSender, _) = await fixture.RegisterAndLoginAsync("sender3", "sender3@example.com", "Test123!");
        var (tokenReceiver, _) = await fixture.RegisterAndLoginAsync("receiver3", "receiver3@example.com", "Test123!");

        _client.DefaultRequestHeaders.Authorization = new("Bearer", tokenSender);
        await _client.PostAsJsonAsync("/friendrequests/send", new CreateFriendRequestCommand("receiver3"));

        _client.DefaultRequestHeaders.Authorization = new("Bearer", tokenReceiver);
        var response = await _client.GetAsync("/friendrequests");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<GetFriendRequestsResponse>();
        content.Should().NotBeNull();
    }
}


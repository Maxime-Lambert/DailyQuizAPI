using DailyQuizAPI.Features.Crosscutting.FriendRequests.Accept;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.Create;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.RemoveFriend;
using DailyQuizAPI.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DailyQuizAPI.IntegrationTests.FriendRequests;

public sealed class RemoveFriendTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    private HttpClient Client => fixture.Client!;

    [Fact]
    public async Task RemoveFriend_ReturnsOk()
    {
        await fixture.ResetDatabaseAsync();
        var unique = Guid.NewGuid().ToString("N")[..8];
        var (tokenSender, _) = await fixture.RegisterAndLoginAsync($"sender_{unique}", $"sender_{unique}@example.com", "Test123!");
        var (tokenReceiver, _) = await fixture.RegisterAndLoginAsync($"receiver_{unique}", $"receiver_{unique}@example.com", "Test123!");

        var senderId = await fixture.GetUserIdByUsernameAsync($"sender_{unique}");

        Client.DefaultRequestHeaders.Authorization = new("Bearer", tokenSender);
        await Client.PostAsJsonAsync("/friendrequests", new CreateFriendRequestCommand($"receiver_{unique}"));

        Client.DefaultRequestHeaders.Authorization = new("Bearer", tokenReceiver);
        await Client.PostAsJsonAsync("/friendrequests/accept", new AcceptFriendRequestCommand(senderId));

        var response = await Client.PostAsJsonAsync("/friendrequests/removefriend", new RemoveFriendCommand(senderId));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

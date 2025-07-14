namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.GetAll;

public sealed record FriendRequestResponse(int Id, string Direction, string UserId, string UserName, string Email);

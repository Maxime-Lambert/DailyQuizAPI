using System.Collections.ObjectModel;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.GetAll;

public sealed record GetFriendRequestsResponse(
    Collection<FriendRequestResponse> Accepted,
    Collection<FriendRequestResponse> Pending
);

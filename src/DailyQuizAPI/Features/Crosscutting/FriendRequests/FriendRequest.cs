using DailyQuizAPI.Features.Crosscutting.Users;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests;

public sealed class FriendRequest
{
    public int Id { get; set; }

    public string RequesterId { get; set; } = default!;
    public User Requester { get; set; } = default!;

    public string ReceiverId { get; set; } = default!;
    public User Receiver { get; set; } = default!;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public bool IsAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
}


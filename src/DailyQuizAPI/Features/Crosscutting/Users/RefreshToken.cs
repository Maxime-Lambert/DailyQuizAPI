namespace DailyQuizAPI.Features.Crosscutting.Users;

public sealed class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = default!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

}


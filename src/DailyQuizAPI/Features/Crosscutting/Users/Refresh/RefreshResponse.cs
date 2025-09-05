namespace DailyQuizAPI.Features.Crosscutting.Users.Refresh;

public sealed record RefreshResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiresAt);

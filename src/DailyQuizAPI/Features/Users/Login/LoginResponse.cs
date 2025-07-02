namespace DailyQuizAPI.Features.Users.Login;

public sealed record LoginResponse(string Token, string RefreshToken);

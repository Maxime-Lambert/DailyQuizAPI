namespace DailyQuizAPI.Features.Crosscutting.Users.Login;

public sealed record LoginResponse(string Token, string RefreshToken);

namespace DailyQuizAPI.Features.Users.Refresh;

public sealed record RefreshCommand(string RefreshToken, string IpAdress);

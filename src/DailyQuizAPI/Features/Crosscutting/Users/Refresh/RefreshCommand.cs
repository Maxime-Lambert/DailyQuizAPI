namespace DailyQuizAPI.Features.Crosscutting.Users.Refresh;

public sealed record RefreshCommand(string RefreshToken, string IpAdress);

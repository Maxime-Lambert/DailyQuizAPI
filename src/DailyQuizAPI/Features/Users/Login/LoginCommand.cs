namespace DailyQuizAPI.Features.Users.Login;

public sealed record LoginCommand(
    string UserName,
    string Password,
    string IpAddress
);
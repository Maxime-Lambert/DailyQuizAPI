namespace DailyQuizAPI.Features.Crosscutting.Users.Login;

public sealed record LoginCommand(
    string UserName,
    string Password
);

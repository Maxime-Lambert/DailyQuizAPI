namespace DailyQuizAPI.Users.Login;

public sealed record LoginCommand(
    string UserName,
    string Password
);
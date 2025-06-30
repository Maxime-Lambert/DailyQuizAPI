namespace DailyQuizAPI.Users.Create;

public sealed record CreateUserCommand(
    string UserName,
    string? Email,
    string Password
);

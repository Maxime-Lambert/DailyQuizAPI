namespace DailyQuizAPI.Features.Users.Create;

public sealed record CreateUserCommand(
    string UserName,
    string? Email,
    string Password
);

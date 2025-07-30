namespace DailyQuizAPI.Features.Crosscutting.Users.ResetPassword;

public sealed record ResetPasswordCommand(string Token, string Password);

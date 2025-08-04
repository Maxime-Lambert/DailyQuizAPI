using DailyQuizAPI.Middlewares;

namespace DailyQuizAPI.Features.Crosscutting.Users.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email, FrontEndNames FrontEndName);

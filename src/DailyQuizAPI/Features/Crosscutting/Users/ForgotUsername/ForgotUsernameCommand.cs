using DailyQuizAPI.Middlewares;

namespace DailyQuizAPI.Features.Crosscutting.Users.ForgotUsername;

public sealed record ForgotUsernameCommand(string Email, FrontEndNames FrontEndName);

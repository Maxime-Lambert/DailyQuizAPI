using DailyQuizAPI.Middlewares;

namespace DailyQuizAPI.Features.Crosscutting.Users.SendContactMessage;

public sealed record SendContactMessageCommand(string Name, string Email, string Message, FrontEndNames FrontEndName);


using DailyQuizAPI.Middlewares;

namespace DailyQuizAPI.Features.Crosscutting.Users.ResendConfirmation;

public sealed record ResendConfirmationCommand(string Email, FrontEndNames FrontEndName);

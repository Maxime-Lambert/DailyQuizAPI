using DailyQuizAPI.Middlewares;

namespace DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;

public sealed record PartialUpdateUserCommand(
    string? UserName,
    string? Email,
    string? LastPassword,
    string? NewPassword,
    ColorblindMode? ColorblindMode,
    KeyboardLayout? KeyboardLayout,
    SmartKeyboardType? SmartKeyboardType,
    bool? PlaysWithDifficultWords,
    FrontEndNames FrontEndName
);

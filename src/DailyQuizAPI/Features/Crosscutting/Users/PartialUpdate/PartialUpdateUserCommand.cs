using DailyQuizAPI.Middlewares;

namespace DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;

public sealed record PartialUpdateUserCommand(
    string? Username,
    string? Email,
    string? LastPassword,
    string? NewPassword,
    ColorblindMode? ColorblindMode,
    KeyboardLayout? KeyboardLayout,
    SmartKeyboardType? SmartKeyboardType,
    FrontEndNames FrontEndName
);

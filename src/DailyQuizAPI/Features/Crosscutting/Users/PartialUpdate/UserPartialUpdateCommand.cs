namespace DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;

public sealed record UserPartialUpdateCommand(
    string? UserName,
    string? Email,
    string? LastPassword,
    string? NewPassword,
    ColorblindMode? ColorblindMode,
    KeyboardLayout? KeyboardLayout,
    SmartKeyboardType? SmartKeyboardType
);

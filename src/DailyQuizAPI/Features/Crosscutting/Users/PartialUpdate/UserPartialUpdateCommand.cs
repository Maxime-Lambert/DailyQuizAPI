namespace DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;

public sealed record UserPartialUpdateCommand(
    string? UserName,
    string? Email,
    string? Password,
    ColorblindMode? ColorblindMode,
    KeyboardLayout? KeyboardLayout,
    SmartKeyboardType? SmartKeyboardType
);

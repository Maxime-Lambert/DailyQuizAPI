namespace DailyQuizAPI.AppSettings.Create;

public sealed record CreateAppSettingCommand(
    string Key,
    string Value
);

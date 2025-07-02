namespace DailyQuizAPI.Features.AppSettings.Create;

public sealed record CreateAppSettingCommand(
    string Key,
    string Value
);

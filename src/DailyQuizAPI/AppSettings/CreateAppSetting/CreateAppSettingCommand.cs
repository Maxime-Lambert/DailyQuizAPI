namespace DailyQuizAPI.AppSettings.CreateAppSetting;

public sealed record CreateAppSettingCommand(
    string Key,
    string Value
);

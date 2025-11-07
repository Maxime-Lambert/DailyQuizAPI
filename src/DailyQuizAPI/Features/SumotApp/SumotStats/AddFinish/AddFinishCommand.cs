namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddFinish;

public sealed record AddFinishCommand(DateOnly Date, bool IsMobile);
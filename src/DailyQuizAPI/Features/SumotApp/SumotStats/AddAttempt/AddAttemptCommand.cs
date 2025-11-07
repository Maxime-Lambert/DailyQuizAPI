namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddAttempt;

public sealed record AddAttemptCommand(DateOnly Date, bool IsMobile);
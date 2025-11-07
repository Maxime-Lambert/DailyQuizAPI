namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddVisit;

public sealed record AddVisitCommand(DateOnly Date, bool IsMobile);

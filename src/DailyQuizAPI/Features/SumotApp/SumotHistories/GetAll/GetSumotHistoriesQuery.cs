namespace DailyQuizAPI.Features.SumotApp.SumotHistories.GetAll;

public sealed record GetSumotHistoriesQuery(DateOnly StartDate, DateOnly EndDate);

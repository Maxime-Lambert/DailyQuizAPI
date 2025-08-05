namespace DailyQuizAPI.Features.SumotApp.SumotHistories.GetAll;

public sealed record GetSumotHistoriesQuery(DateOnly MinDate, DateOnly MaxDate);

namespace DailyQuizAPI.Features.SumotApp.Sumots.GetAll;

public sealed record GetSumotsQuery(int? DatabaseVersion, DateOnly? Day);
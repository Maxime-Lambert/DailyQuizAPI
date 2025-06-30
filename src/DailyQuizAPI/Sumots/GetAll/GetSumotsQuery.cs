namespace DailyQuizAPI.Sumots.GetAll;

public sealed record GetSumotsQuery(int? DatabaseVersion, DateOnly? Day);
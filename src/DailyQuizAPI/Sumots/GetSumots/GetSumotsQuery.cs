namespace DailyQuizAPI.Sumots.GetSumots;

public sealed record GetSumotsQuery(int? DatabaseVersion, DateOnly? Day);
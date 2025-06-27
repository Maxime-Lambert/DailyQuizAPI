namespace DailyQuizAPI.Sumots.GetSumots;

public sealed record GetSumotsResponse(int Id, string Word, DateOnly? Day);

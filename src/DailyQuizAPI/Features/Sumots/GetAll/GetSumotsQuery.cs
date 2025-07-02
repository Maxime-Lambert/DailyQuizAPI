namespace DailyQuizAPI.Features.Sumots.GetAll;

public sealed record GetSumotsQuery(int? DatabaseVersion, DateOnly? Day);
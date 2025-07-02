namespace DailyQuizAPI.Features.Sumots.GetAll;

public sealed record GetSumotsResponse(int Id, string Word, DateOnly? Day);

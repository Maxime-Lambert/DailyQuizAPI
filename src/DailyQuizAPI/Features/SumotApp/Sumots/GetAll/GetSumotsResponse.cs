namespace DailyQuizAPI.Features.SumotApp.Sumots.GetAll;

public sealed record GetSumotsResponse(int Id, string Word, DateOnly? Day, string Definition, string DefinitionWord, bool IsDifficult);

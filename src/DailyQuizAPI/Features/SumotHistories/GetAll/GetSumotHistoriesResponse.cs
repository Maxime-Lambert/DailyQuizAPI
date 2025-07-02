namespace DailyQuizAPI.Features.SumotHistories.GetAll;

public sealed record GetSumotHistoriesResponse(
    int Id,
    string Word,
    IReadOnlyCollection<string> Tries,
    DateOnly Day,
    int? Ranking
);

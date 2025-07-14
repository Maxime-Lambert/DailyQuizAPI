namespace DailyQuizAPI.Features.SumotApp.SumotHistories.GetAll;

public sealed record GetSumotHistoriesResponse(
    int Id,
    string Word,
    IReadOnlyCollection<string> Tries,
    int? Ranking,
    string PlayerId
);
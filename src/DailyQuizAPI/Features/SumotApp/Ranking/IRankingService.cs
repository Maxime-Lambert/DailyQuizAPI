namespace DailyQuizAPI.Features.SumotApp.Ranking;

public interface IRankingService
{
    Task RecalculateRankingsAsync(string userId, CancellationToken ct);
}

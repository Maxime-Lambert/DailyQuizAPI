using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Features.SumotApp.Ranking;
using DailyQuizAPI.Persistence;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.Add;

public sealed class AddSumotHistoriesCommandHandler(QuizContext quizContext, IRankingService rankingService, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly IRankingService _rankingService = rankingService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(AddSumotHistoriesCommand command, ClaimsPrincipal principal, CancellationToken ct)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

        foreach (var history in command.Histories)
        {
            var newHistory = new SumotHistory
            {
                UserId = userId,
                Word = history.Word
            };
            newHistory.AddTries(history.Tries);

            await _quizContext.SumotHistories.AddAsync(newHistory, ct).ConfigureAwait(false);
        }

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

        await _rankingService.RecalculateRankingsAsync(userId, ct).ConfigureAwait(false);

        _cacheService.RemoveByPrefix($"sumotHistories:{userId}:");
    }
}


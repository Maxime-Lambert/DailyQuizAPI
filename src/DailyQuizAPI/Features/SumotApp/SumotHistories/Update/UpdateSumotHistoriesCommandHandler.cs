using DailyQuizAPI.Common.Exceptions;
using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Features.SumotApp.Ranking;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.Update;

public sealed class UpdateSumotHistoriesCommandHandler(QuizContext quizContext, IRankingService rankingService, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly IRankingService _rankingService = rankingService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(UpdateSumotHistoriesCommand command, ClaimsPrincipal principal, CancellationToken ct)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new NotFoundException("Utilisateur introuvable dans les revendications.");

        _cacheService.RemoveByPrefix($"sumotHistories:{userId}:");

        foreach (var history in command.Histories)
        {
            var currentHistory = await _quizContext.SumotHistories
            .FirstOrDefaultAsync(h => h.UserId == userId && h.Word == history.Word, ct)
            .ConfigureAwait(false);

            if (currentHistory != null)
            {
                currentHistory.ClearTries();
                currentHistory.AddTries(new Collection<string>([.. history.Tries]));
                _quizContext.SumotHistories.Update(currentHistory);
                await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);
                if (currentHistory.IsFinished)
                    await _rankingService.RecalculateRankingsAsync(userId, ct).ConfigureAwait(false);
                return;
            }

            var newHistory = new SumotHistory
            {
                UserId = userId,
                Word = history.Word,
            };
            newHistory.AddTries(new Collection<string>([.. history.Tries]));

            await _quizContext.SumotHistories.AddAsync(newHistory, ct).ConfigureAwait(false);

            await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

            if (newHistory.IsFinished)
                await _rankingService.RecalculateRankingsAsync(userId, ct).ConfigureAwait(false);
        }


    }
}
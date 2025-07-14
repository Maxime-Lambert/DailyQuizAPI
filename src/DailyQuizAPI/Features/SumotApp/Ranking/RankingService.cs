using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Features.SumotApp.Ranking;

public sealed class RankingService(QuizContext quizContext) : IRankingService
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task RecalculateRankingsAsync(string userId, CancellationToken ct)
    {
        var words = await _quizContext.SumotHistories
            .Where(h => h.UserId == userId)
            .Select(h => h.Word)
            .Distinct()
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var scope = await GetFriendsAsync(userId, ct).ConfigureAwait(false);

        foreach (var word in words)
        {
            var scopedHistories = _quizContext.SumotHistories
                .Where(h => h.Word == word && scope.Contains(h.UserId))
                .AsEnumerable()
                .Where(h => h.Tries != null)
                .OrderBy(h => h.Tries!.Count)
                .ToList();

            var currentRank = 1;
            int? previousTriesCount = null;

            for (var i = 0; i < scopedHistories.Count; i++)
            {
                var currentCount = scopedHistories[i].Tries!.Count;

                if (currentCount != previousTriesCount)
                    currentRank = i + 1;

                scopedHistories[i].Ranking = currentRank;
                previousTriesCount = currentCount;
            }

            _quizContext.SumotHistories.UpdateRange(scopedHistories);
        }

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private async Task<List<string>> GetFriendsAsync(string userId, CancellationToken ct)
    {
        var friendIds = await _quizContext.FriendRequests
            .Where(fr => fr.IsAccepted && (fr.RequesterId == userId || fr.ReceiverId == userId))
            .Select(fr => fr.RequesterId == userId ? fr.ReceiverId : fr.RequesterId)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return [.. friendIds, userId];
    }
}


using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotHistories.Add;

public sealed class AddSumotHistoryCommandHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(AddSumotHistoryCommand command, ClaimsPrincipal principal, CancellationToken ct)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var newHistory = new SumotHistory
        {
            UserId = userId,
            Word = command.Word,
            Day = command.Day,
        };
        newHistory.AddTries(command.Tries);

        _quizContext.SumotHistories.Add(newHistory);

        var friendIds = await _quizContext.FriendRequests
            .Where(fr => fr.IsAccepted &&
                (fr.RequesterId == userId || fr.ReceiverId == userId))
            .Select(fr => fr.RequesterId == userId ? fr.ReceiverId : fr.RequesterId)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var allUserIds = friendIds.Append(userId).ToList();

        var histories = await _quizContext.SumotHistories
            .Where(h => allUserIds.Contains(h.UserId) && h.Word == command.Word)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var sortedHistories = histories
            .Where(h => h.Tries != null)
            .OrderBy(h => h.Tries!.Count)
            .ToList();

        var nextRank = 1;
        for (var i = 0; i < sortedHistories.Count; i++)
        {
            if (i > 0 && sortedHistories[i].Tries.Count != sortedHistories[i - 1].Tries.Count)
                nextRank++;
            sortedHistories[i].Ranking = nextRank;
        }

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}


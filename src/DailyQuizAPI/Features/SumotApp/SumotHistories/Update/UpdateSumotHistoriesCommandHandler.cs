using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.Update;

public sealed class UpdateSumotHistoriesCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(UpdateSumotHistoriesCommand command, ClaimsPrincipal principal, CancellationToken ct)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        foreach (var history in command.Histories)
        {
            var currentHistory = await _quizContext.SumotHistories
                .Include(h => h.Tries)
                .FirstOrDefaultAsync(h => h.UserId == userId && h.Word == history.Word, ct)
                .ConfigureAwait(false);

            if (currentHistory is not null)
            {
                if(!command.Overwrite.HasValue || command.Overwrite!.Value)
                {
                    currentHistory.ReplaceTries(history.Tries);
                    currentHistory.Won = history.Won;
                    _quizContext.SumotHistories.Update(currentHistory);
                }
            }
            else
            {
                var newHistory = new SumotHistory
                {
                    UserId = userId,
                    Word = history.Word,
                    Won = history.Won,
                };
                newHistory.ReplaceTries(history.Tries);
                await _quizContext.SumotHistories.AddAsync(newHistory, ct).ConfigureAwait(false);
            }
        }

        var friendIds = await _quizContext.FriendRequests
            .Where(fr => fr.IsAccepted && (fr.RequesterId == userId || fr.ReceiverId == userId))
            .Select(fr => fr.RequesterId == userId ? fr.ReceiverId : fr.RequesterId)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        friendIds.Add(userId);

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);
        foreach (var id in friendIds)
        {
            _cacheService.RemoveByPrefix($"sumotHistories:{userId}");
        }
    }
}
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

        bool overwrite = command.Overwrite ?? false;

        foreach (var h in command.Histories)
        {
            var existing = await _quizContext.SumotHistories
                .Include(x => x.Tries)
                .Where(x => x.UserId == userId && x.Word == h.Word)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);

            if (existing is null)
            {
                var newHistory = new SumotHistory
                {
                    UserId = userId,
                    Word = h.Word,
                    Won = h.Won
                };
                newHistory.ReplaceTries(h.Tries);

                await _quizContext.SumotHistories.AddAsync(newHistory, ct).ConfigureAwait(false);
            }
            else
            {
                if (!overwrite)
                    continue;

                int newCount = h.Tries.Count;
                int oldCount = existing.Tries.Count;

                if (newCount > oldCount)
                {
                    existing.Won = h.Won;
                    existing.ReplaceTries(h.Tries);
                }
            }
        }

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

        var friendIds = await _quizContext.FriendRequests
            .Where(fr => fr.IsAccepted && (fr.RequesterId == userId || fr.ReceiverId == userId))
            .Select(fr => fr.RequesterId == userId ? fr.ReceiverId : fr.RequesterId)
            .ToListAsync(ct).ConfigureAwait(false);

        friendIds.Add(userId);
        foreach (var id in friendIds)
            _cacheService.RemoveByPrefix($"sumotHistories:{id}");
    }
}

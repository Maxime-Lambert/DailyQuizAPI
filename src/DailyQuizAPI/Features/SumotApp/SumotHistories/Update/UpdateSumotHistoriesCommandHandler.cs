using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            var newHistory = new SumotHistory
            {
                UserId = userId,
                Word = history.Word,
                Won = history.Won
            };
            newHistory.ReplaceTries(history.Tries);

            await _quizContext.SumotHistories.AddAsync(newHistory, ct).ConfigureAwait(false);

            try
            {
                await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                var existing = await _quizContext.SumotHistories
                    .Include(h => h.Tries)
                    .FirstAsync(h => h.UserId == userId && h.Word == history.Word, ct)
                    .ConfigureAwait(false);

                if ((!command.Overwrite.HasValue || command.Overwrite.Value) && existing.Tries.Count < history.Tries.Count)
                {
                    existing.ReplaceTries(history.Tries);
                    existing.Won = history.Won;
                }

                await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);
            }
        }

        var friendIds = await _quizContext.FriendRequests
            .Where(fr => fr.IsAccepted && (fr.RequesterId == userId || fr.ReceiverId == userId))
            .Select(fr => fr.RequesterId == userId ? fr.ReceiverId : fr.RequesterId)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        friendIds.Add(userId);

        foreach (var id in friendIds)
        {
            _cacheService.RemoveByPrefix($"sumotHistories:{id}");
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException pg
               && pg.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}

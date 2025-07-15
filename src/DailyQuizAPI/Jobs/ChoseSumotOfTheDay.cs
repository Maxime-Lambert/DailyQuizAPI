using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Jobs;

public sealed class ChoseSumotOfTheDay(QuizContext db, ILogger<ChoseSumotOfTheDay> logger)
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existing = await db.Sumots.AnyAsync(s => s.Day == today, ct).ConfigureAwait(false);
        if (existing) return;

        var candidate = await db.Sumots
            .Where(s => s.Day == null)
            .OrderBy(_ => Guid.NewGuid())
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (candidate is null)
        {
            logger.LogNoSumotPossible(today);
            return;
        }

        candidate.Day = today;
        await db.SaveChangesAsync(ct).ConfigureAwait(false);

        logger.LogSumotChosen(candidate.Word, today);
    }
}

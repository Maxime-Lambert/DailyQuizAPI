using DailyQuizAPI.Features.SumotApp.Sumots;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Persistence;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DailyQuizAPI.Jobs;

[AutomaticRetry(Attempts = 6, DelaysInSeconds = new[] { 600, 600, 600, 600, 600, 600 })]
public sealed class ChoseSumotOfTheDay(QuizContext db, ILogger<ChoseSumotOfTheDay> logger)
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        var parisTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, parisTz));

        if (await db.Sumots.AnyAsync(s => s.Day == today, ct).ConfigureAwait(false))
            return;

        var candidate = await PickRandomCandidateAsync(ct).ConfigureAwait(false);

        if (candidate is null)
        {
            await db.Sumots.ExecuteUpdateAsync(
                setters => setters.SetProperty(s => s.Day, (DateOnly?)null),
                ct
            ).ConfigureAwait(false);

            candidate = await PickRandomCandidateAsync(ct).ConfigureAwait(false);

            if (candidate is null)
            {
                logger.LogNoSumotPossible(today);
                return;
            }
        }

        candidate.Day = today;
        await db.SaveChangesAsync(ct).ConfigureAwait(false);

        logger.LogSumotChosen(candidate.Word, today);
    }

    private async Task<Sumot?> PickRandomCandidateAsync(CancellationToken ct)
    {
        var count = await db.Sumots
            .Where(s => s.Day == null && !s.IsDifficult)
            .CountAsync(ct)
            .ConfigureAwait(false);

        if (count == 0) return null;

        int randomIndex = RandomNumberGenerator.GetInt32(count);

        return await db.Sumots
            .Where(s => s.Day == null && !s.IsDifficult)
            .Skip(randomIndex)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }
}

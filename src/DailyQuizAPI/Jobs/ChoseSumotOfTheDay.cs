using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Features.SumotApp.Sumots;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Persistence;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DailyQuizAPI.Jobs;

[AutomaticRetry(Attempts = 6, DelaysInSeconds = new[] { 600, 600, 600, 600, 600, 600 })]
public sealed class ChoseSumotOfTheDay(QuizContext db, ILogger<ChoseSumotOfTheDay> logger, ICacheService cacheService)
{
    private readonly QuizContext _db = db;
    private readonly ILogger<ChoseSumotOfTheDay> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;

    public async Task RunAsync(CancellationToken ct = default)
    {
        var parisTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, parisTz));

        if (await _db.Sumots.AnyAsync(s => s.Day == today, ct).ConfigureAwait(false))
            return;

        var AreTwoLastSumotsLengthFive = await _db.Sumots
            .Where(s => s.Day != null)
            .OrderByDescending(s => s.Day)
            .Take(2)
            .AllAsync(s => s.Word.Length == 5, ct)
            .ConfigureAwait(false);

        var sumotLength = AreTwoLastSumotsLengthFive ? 6 : 5;

        var candidate = await PickRandomCandidateAsync(sumotLength, ct).ConfigureAwait(false);

        if (candidate is null)
        {
            await _db.Sumots.Where(s => s.Word.Length == sumotLength)
                .ExecuteUpdateAsync(
                setters => setters.SetProperty(s => s.Day, (DateOnly?)null),
                ct
            ).ConfigureAwait(false);

            candidate = await PickRandomCandidateAsync(sumotLength, ct).ConfigureAwait(false);

            if (candidate is null)
            {
                _logger.LogNoSumotPossible(today);
                return;
            }
        }

        candidate.Day = today;
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
        _cacheService.RemoveByPrefix("sumots:");

        _logger.LogSumotChosen(candidate.Word, today);
    }

    private async Task<Sumot?> PickRandomCandidateAsync(int sumotLength, CancellationToken ct)
    {
        var count = await _db.Sumots
            .Where(s => s.Day == null && !s.IsDifficult && s.Word.Length == sumotLength)
            .CountAsync(ct)
            .ConfigureAwait(false);

        if (count == 0) return null;

        int randomIndex = RandomNumberGenerator.GetInt32(count);

        return await _db.Sumots
            .Where(s => s.Day == null && !s.IsDifficult && s.Word.Length == sumotLength)
            .Skip(randomIndex)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }
}

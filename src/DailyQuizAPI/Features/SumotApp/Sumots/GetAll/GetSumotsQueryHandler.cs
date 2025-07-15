using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DailyQuizAPI.Features.SumotApp.Sumots.GetAll;

public sealed class GetSumotsQueryHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<GetSumotsResponseList> Handle(GetSumotsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = request.DatabaseVersion switch
        {
            null when request.Day is null => "sumots:all",
            null => $"sumots:day:{request.Day}",
            _ when request.Day is null => $"sumots:version:{request.DatabaseVersion}",
            _ => $"sumots:version:{request.DatabaseVersion}:day:{request.Day}"
        };

        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            var databaseVersion = await _quizContext.AppSettings
                .FirstOrDefaultAsync(a => a.Key == "DatabaseVersion", cancellationToken)
                .ConfigureAwait(false);

            if (databaseVersion?.Value is null)
                throw new InvalidOperationException("DatabaseVersion setting is missing or invalid.");

            var version = int.Parse(databaseVersion.Value, CultureInfo.InvariantCulture);

            if (request.DatabaseVersion is null && request.Day is null)
            {
                var all = await _quizContext.Sumots
                    .Select(s => new GetSumotsResponse(
                        s.Id,
                        s.Word ?? string.Empty,
                        s.Day))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new GetSumotsResponseList(new ReadOnlyCollection<GetSumotsResponse>(all), version);
            }

            if (request.DatabaseVersion < version)
            {
                // Calculer la différence entre les versions ?
                var all = await _quizContext.Sumots
                    .Select(s => new GetSumotsResponse(
                        s.Id,
                        s.Word ?? string.Empty,
                        s.Day))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new GetSumotsResponseList(new ReadOnlyCollection<GetSumotsResponse>(all), version);
            }

            if (request.DatabaseVersion > version)
                throw new InvalidOperationException("Problème de cohérence : la version donnée est supérieure à celle de la base");

            if (request.Day is null)
            {
                var all = await _quizContext.Sumots
                    .Select(s => new GetSumotsResponse(
                        s.Id,
                        s.Word ?? string.Empty,
                        s.Day))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new GetSumotsResponseList(new ReadOnlyCollection<GetSumotsResponse>(all), version);
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (request.Day > today)
                throw new InvalidOperationException("La date demandée est dans le futur.");

            var byDay = await _quizContext.Sumots
                .Where(s => s.Day >= request.Day)
                .OrderBy(s => s.Day)
                .Select(s => new GetSumotsResponse(
                    s.Id,
                    s.Word ?? string.Empty,
                    s.Day))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new GetSumotsResponseList(new ReadOnlyCollection<GetSumotsResponse>(byDay), version);
        }, TimeSpan.FromMinutes(10)).ConfigureAwait(false);
    }
}

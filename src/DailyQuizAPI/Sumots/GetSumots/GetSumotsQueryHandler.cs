using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DailyQuizAPI.Sumots.GetSumots;

public sealed class GetSumotsQueryHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task<List<GetSumotsResponse>> Handle(GetSumotsQuery request, CancellationToken cancellationToken)
    {
        if (request.DatabaseVersion is null)
        {
            return await _quizContext.Sumots
                .Select(s => new GetSumotsResponse(
                    s.Id,
                    s.Word ?? string.Empty,
                    s.Day
                ))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        var databaseVersion = await _quizContext.AppSettings
            .FirstOrDefaultAsync(a => a.Key == "DatabaseVersion", cancellationToken)
            .ConfigureAwait(false);

        if (databaseVersion?.Value is null)
        {
            throw new InvalidOperationException("DatabaseVersion setting is missing or invalid.");
        }

        if (request.DatabaseVersion < int.Parse(databaseVersion.Value, CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException("TODO : Calculer la différence entre les versions ?");
        }

        if (request.DatabaseVersion > int.Parse(databaseVersion.Value, CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException("Problème de cohérence : la version donnée est supérieure à la version de la base de données");
        }

        if (request.Day is null)
        {
            return await _quizContext.Sumots
                .Select(s => new GetSumotsResponse(
                    s.Id,
                    s.Word ?? string.Empty,
                    s.Day
                ))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (request.Day > today)
        {
            throw new InvalidOperationException("La date demandée est dans le futur.");
        }

        return await _quizContext.Sumots
            .Where(s => s.Day >= request.Day)
            .OrderBy(s => s.Day)
            .Select(s => new GetSumotsResponse(
                s.Id,
                s.Word ?? string.Empty,
                s.Day
            ))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

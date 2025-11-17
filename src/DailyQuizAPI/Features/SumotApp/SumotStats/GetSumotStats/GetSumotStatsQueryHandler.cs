using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Features.SumotApp.SumotStats.GetSumotStats;

public class GetSumotStatsQueryHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task<List<SumotStat>> Handle(CancellationToken cancellationToken)
    {
        var stats = await _quizContext.SumotStats
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return stats;
    }
}

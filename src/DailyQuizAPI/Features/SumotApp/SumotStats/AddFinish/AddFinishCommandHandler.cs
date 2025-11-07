using DailyQuizAPI.Features.SumotApp.SumotStats.AddVisit;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddFinish;

public sealed class AddFinishCommandHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;
    public async Task Handle(AddFinishCommand command, CancellationToken cancellationToken)
    {
        var stat = await _quizContext.SumotStats
            .FirstOrDefaultAsync(s => s.Date == command.Date && s.IsMobile == command.IsMobile, cancellationToken)
            .ConfigureAwait(false);
        if (stat is null)
        {
            stat = new SumotStat
            {
                Date = command.Date,
                IsMobile = command.IsMobile,
                Visits = 0,
                Attempts = 0,
                Finishes = 1
            };
            await _quizContext.SumotStats.AddAsync(stat, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            stat.Finishes += 1;
            _quizContext.SumotStats.Update(stat);
        }
        await _quizContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

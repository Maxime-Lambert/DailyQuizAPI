using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddFinish;

public sealed class AddFinishCommandHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(AddFinishCommand command, CancellationToken cancellationToken)
    {
        await _quizContext.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO ""SumotStats"" (""Date"", ""IsMobile"", ""Visits"", ""Attempts"", ""Finishes"")
            VALUES ({command.Date}, {command.IsMobile}, 0, 0, 1)
            ON CONFLICT (""Date"", ""IsMobile"")
            DO UPDATE SET ""Finishes"" = ""SumotStats"".""Finishes"" + 1;
            ", cancellationToken).ConfigureAwait(false);
    }
}

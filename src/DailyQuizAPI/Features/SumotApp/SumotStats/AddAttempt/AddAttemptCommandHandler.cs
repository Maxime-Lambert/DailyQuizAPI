using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddAttempt;

public sealed class AddAttemptCommandHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(AddAttemptCommand command, CancellationToken cancellationToken)
    {
        await _quizContext.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO ""SumotStats"" (""Date"", ""IsMobile"", ""Visits"", ""Attempts"", ""Finishes"")
            VALUES ({command.Date}, {command.IsMobile}, 0, 1, 0)
            ON CONFLICT (""Date"", ""IsMobile"")
            DO UPDATE SET ""Attempts"" = ""SumotStats"".""Attempts"" + 1;
            ", cancellationToken).ConfigureAwait(false);
    }
}


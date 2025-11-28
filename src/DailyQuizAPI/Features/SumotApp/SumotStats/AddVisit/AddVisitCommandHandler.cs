using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddVisit;

public sealed class AddVisitCommandHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(AddVisitCommand command, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        await _quizContext.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO ""SumotStats"" (""Date"", ""IsMobile"", ""Visits"", ""Attempts"", ""Finishes"")
            VALUES ({today}, {command.IsMobile}, 1, 0, 0)
            ON CONFLICT (""Date"", ""IsMobile"")
            DO UPDATE SET ""Visits"" = ""SumotStats"".""Visits"" + 1;
            ", cancellationToken).ConfigureAwait(false);
    }
}

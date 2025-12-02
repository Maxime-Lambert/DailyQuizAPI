using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.Update;

public sealed class UpdateSumotHistoriesCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(UpdateSumotHistoriesCommand command, ClaimsPrincipal principal, CancellationToken ct)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        foreach (var history in command.Histories)
        {
            await _quizContext.Database.ExecuteSqlAsync($@"
                insert into ""SumotHistories"" (""UserId"", ""Word"", ""Won"", ""Tries"")
                values ({userId}, {history.Word}, {history.Won}, {JsonConvert.SerializeObject(history.Tries)})
                on conflict (""UserId"", ""Word"")
                do update set 
                    ""Won"" = EXCLUDED.""Won"",
                    ""Tries"" = EXCLUDED.""Tries"";
                ", ct).ConfigureAwait(false);
        }

        var friendIds = await _quizContext.FriendRequests
            .Where(fr => fr.IsAccepted && (fr.RequesterId == userId || fr.ReceiverId == userId))
            .Select(fr => fr.RequesterId == userId ? fr.ReceiverId : fr.RequesterId)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        friendIds.Add(userId);

        foreach (var id in friendIds)
        {
            _cacheService.RemoveByPrefix($"sumotHistories:{id}");
        }
    }
}

using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.GetAll;

public class GetSumotHistoriesQueryHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<List<GetSumotHistoriesResponse>> Handle(GetSumotHistoriesQuery query, ClaimsPrincipal claims, CancellationToken ct)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        if ((query.MaxDate.ToDateTime(TimeOnly.MinValue) - query.MinDate.ToDateTime(TimeOnly.MinValue)) > TimeSpan.FromDays(30))
            throw new InvalidOperationException("La plage de dates ne peut pas dépasser 30 jours");

        var cacheKey = $"sumotHistories:{userId}:minDate:{query.MinDate}:maxDate:{query.MaxDate}";

        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            var friendIds = await _quizContext.FriendRequests
                .Where(fr => fr.IsAccepted && (fr.RequesterId == userId || fr.ReceiverId == userId))
                .Select(fr => fr.RequesterId == userId ? fr.ReceiverId : fr.RequesterId)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            friendIds.Add(userId);

            return await (
                from history in _quizContext.SumotHistories
                join sumot in _quizContext.Sumots
                    on history.Word equals sumot.Word
                join user in _quizContext.Users
                    on history.UserId equals user.Id
                where friendIds.Contains(history.UserId)
                   && sumot.Day >= query.MinDate
                   && sumot.Day <= query.MaxDate
                orderby sumot.Day descending,
                 history.Tries.Count ascending,
                 history.Won descending
                select new GetSumotHistoriesResponse(
                    history.Id,
                    history.Word,
                    history.Tries
                        .OrderBy(t => t.Id)
                        .Select(t => t.Value).ToList(),
                    history.Won,
                    user.UserName!
                )).ToListAsync(ct).ConfigureAwait(false);
        }, TimeSpan.FromMinutes(10)).ConfigureAwait(false);
    }
}

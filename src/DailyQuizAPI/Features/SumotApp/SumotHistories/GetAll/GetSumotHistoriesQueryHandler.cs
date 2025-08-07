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
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if ((query.MaxDate.ToDateTime(TimeOnly.MinValue) - query.MinDate.ToDateTime(TimeOnly.MinValue)) > TimeSpan.FromDays(30))
            throw new ArgumentException("La plage de dates ne peut pas dépasser 30 jours.", nameof(query));
        var cacheKey = $"sumotHistories:{userId}:minDate:{query.MinDate}:maxDate:{query.MaxDate}";

        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            var friendIds = await _quizContext.FriendRequests
                .Where(fr => fr.IsAccepted && (fr.RequesterId == userId || fr.ReceiverId == userId))
                .Select(fr => fr.RequesterId == userId ? fr.ReceiverId : fr.RequesterId)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            friendIds.Add(userId);

            var histories = await (
                from history in _quizContext.SumotHistories
                join sumot in _quizContext.Sumots
                    on history.Word equals sumot.Word
                where friendIds.Contains(history.UserId)
                   && sumot.Day >= query.MinDate
                   && sumot.Day <= query.MaxDate
                orderby sumot.Day
                select history
            ).ToListAsync(ct).ConfigureAwait(false);

            return histories.Select(h => new GetSumotHistoriesResponse(
                h.Id, h.Word, h.Tries, h.Ranking, h.User.UserName!)).ToList();
        }, TimeSpan.FromMinutes(10)).ConfigureAwait(false);
    }
}

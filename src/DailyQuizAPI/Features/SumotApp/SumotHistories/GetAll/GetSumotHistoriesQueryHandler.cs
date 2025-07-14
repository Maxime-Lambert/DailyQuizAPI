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
        var cacheKey = $"sumotHistories:{userId}:page:{query.Page}:size:{query.PageSize}";

        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            var friendIds = await _quizContext.FriendRequests
                .Where(fr => fr.IsAccepted && (fr.RequesterId == userId || fr.ReceiverId == userId))
                .Select(fr => fr.RequesterId == userId ? fr.ReceiverId : fr.RequesterId)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            friendIds.Add(userId);

            var histories = await _quizContext.SumotHistories
                .Where(h => friendIds.Contains(h.UserId))
                .OrderBy(h => _quizContext.Sumots.FirstOrDefault(s => s.Word == h.Word)!.Day)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            return histories.Select(h => new GetSumotHistoriesResponse(
                h.Id, h.Word, h.Tries, h.Ranking, h.UserId)).ToList();
        }, TimeSpan.FromMinutes(10)).ConfigureAwait(false);
    }
}

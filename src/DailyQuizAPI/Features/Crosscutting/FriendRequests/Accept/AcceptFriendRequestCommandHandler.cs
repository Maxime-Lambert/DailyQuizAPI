using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Features.SumotApp.Ranking;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.Accept;

public class AcceptFriendRequestCommandHandler(QuizContext quizContext, IRankingService rankingService, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly IRankingService _rankingService = rankingService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(AcceptFriendRequestCommand command, ClaimsPrincipal claims, CancellationToken ct)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var userFriends = _quizContext.FriendRequests.Where(fr => fr.RequesterId == userId || fr.ReceiverId == userId);
        var userFriendCount = await userFriends.CountAsync(ct).ConfigureAwait(false);
        if (userFriendCount == 20)
            throw new InvalidOperationException("You cannot have more than 20 friends.");

        var friendRequest = await _quizContext.FriendRequests.FirstOrDefaultAsync(fr =>
                fr.RequesterId == userId && fr.ReceiverId == command.TargetUserId ||
                fr.RequesterId == command.TargetUserId && fr.ReceiverId == userId,
            ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Friend request not found.");

        friendRequest.IsAccepted = true;
        friendRequest.AcceptedAt = DateTime.UtcNow;

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

        await _rankingService.RecalculateRankingsAsync(friendRequest.RequesterId, ct).ConfigureAwait(false);
        await _rankingService.RecalculateRankingsAsync(friendRequest.ReceiverId, ct).ConfigureAwait(false);

        _cacheService.Remove($"friendRequests:{userId}");
        _cacheService.Remove($"friendRequests:{command.TargetUserId}");
        _cacheService.Remove($"sumotHistories:{userId}");
        _cacheService.Remove($"sumotHistories:{command.TargetUserId}");
    }
}

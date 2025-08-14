using DailyQuizAPI.Common.Exceptions;
using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Features.SumotApp.Ranking;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.RemoveFriend;

public sealed class RemoveFriendCommandHandler(QuizContext quizContext, IRankingService rankingService, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly IRankingService _rankingService = rankingService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task HandleAsync(RemoveFriendCommand command, ClaimsPrincipal claims, CancellationToken ct)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new NotFoundException("Utilisateur introuvable dans les revendications.");

        var friendRequest = await _quizContext.FriendRequests
            .FirstOrDefaultAsync(fr =>
                fr.IsAccepted &&
                (fr.RequesterId == userId && fr.ReceiverId == command.TargetUserId ||
                 fr.RequesterId == command.TargetUserId && fr.ReceiverId == userId), ct)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(nameof(FriendRequest), command.TargetUserId);

        _quizContext.FriendRequests.Remove(friendRequest);

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

        await _rankingService.RecalculateRankingsAsync(friendRequest.RequesterId, ct).ConfigureAwait(false);
        await _rankingService.RecalculateRankingsAsync(friendRequest.ReceiverId, ct).ConfigureAwait(false);

        _cacheService.Remove($"friendRequests:{userId}");
        _cacheService.Remove($"friendRequests:{command.TargetUserId}");
        _cacheService.Remove($"sumotHistories:{userId}");
        _cacheService.Remove($"sumotHistories:{command.TargetUserId}");
    }
}

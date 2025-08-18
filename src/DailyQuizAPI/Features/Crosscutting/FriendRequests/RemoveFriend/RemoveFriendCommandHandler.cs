using DailyQuizAPI.Common.Exceptions;
using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.RemoveFriend;

public sealed class RemoveFriendCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
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

        _cacheService.RemoveByPrefix($"friendRequests:{userId}");
        _cacheService.RemoveByPrefix($"friendRequests:{command.TargetUserId}");
        _cacheService.RemoveByPrefix($"sumotHistories:{userId}");
        _cacheService.RemoveByPrefix($"sumotHistories:{command.TargetUserId}");
    }
}
